using apitextil.Models;
using apitextil.Services;
using apitextilECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class ComprobantesController : ControllerBase
{
    private readonly IVentaService _ventaService;
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ComprobantesController> _logger;

    public ComprobantesController(
        IVentaService ventaService,
        IConfiguration config,
        IHttpClientFactory httpClientFactory,
        ILogger<ComprobantesController> logger)
    {
        _ventaService = ventaService;
        _config = config;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public class EmitirComprobanteDto
    {
        public int VentaId { get; set; }
        public int TipoComprobante { get; set; } // 1=Factura, 2=Boleta
        public int? NumeroForzado { get; set; }

        // Datos para boleta (personas naturales)
        public string? ClienteDNI { get; set; }
        public string? ClienteNombres { get; set; }
        public string? ClienteApellidos { get; set; }

        // Datos para factura (empresas)
        public string? RUC { get; set; }
        public string? RazonSocial { get; set; }
    }

    [HttpPost("emitir")]
    public async Task<IActionResult> Emitir([FromBody] EmitirComprobanteDto dto)
    {
        try
        {
            _logger.LogInformation("🧾 Iniciando emisión de comprobante para VentaId: {VentaId}", dto.VentaId);

            // 1️⃣ Validaciones iniciales
            if (dto.VentaId <= 0)
            {
                return BadRequest(new { ok = false, mensaje = "VentaId inválido" });
            }

            if (dto.TipoComprobante != 1 && dto.TipoComprobante != 2)
            {
                return BadRequest(new { ok = false, mensaje = "Tipo de comprobante inválido. Debe ser 1 (Factura) o 2 (Boleta)" });
            }

            // Validar datos según tipo de comprobante
            if (dto.TipoComprobante == 1) // Factura
            {
                if (string.IsNullOrWhiteSpace(dto.RUC) || string.IsNullOrWhiteSpace(dto.RazonSocial))
                {
                    return BadRequest(new { ok = false, mensaje = "Para factura se requiere RUC y Razón Social" });
                }
            }
            else // Boleta
            {
                if (string.IsNullOrWhiteSpace(dto.ClienteNombres) || string.IsNullOrWhiteSpace(dto.ClienteApellidos))
                {
                    return BadRequest(new { ok = false, mensaje = "Para boleta se requiere nombres y apellidos del cliente" });
                }
            }

            // 2️⃣ Obtener venta completa
            var venta = await _ventaService.ObtenerVentaCompletaAsync(dto.VentaId);
            if (venta == null)
            {
                _logger.LogWarning("❌ No se encontró la venta {VentaId}", dto.VentaId);
                return NotFound(new { ok = false, mensaje = $"No existe la venta {dto.VentaId}" });
            }

            _logger.LogInformation("✅ Venta encontrada. Detalles: {DetallesCount}", venta.Detalles?.Count ?? 0);

            // 3️⃣ Asignar serie según tipo de comprobante
            string? serie = dto.TipoComprobante == 1
                ? _config["NubeFact:SerieFactura"] // Factura
                : _config["NubeFact:SerieBoleta"]; // Boleta

            if (string.IsNullOrEmpty(serie))
            {
                _logger.LogError("❌ No se encontró configuración de serie para tipo {TipoComprobante}", dto.TipoComprobante);
                return BadRequest(new { ok = false, mensaje = "Error de configuración: Serie no encontrada" });
            }

            // 4️⃣ Número de comprobante
            int numero = dto.NumeroForzado.HasValue && dto.NumeroForzado.Value > 0
                         ? dto.NumeroForzado.Value
                         : await _ventaService.ObtenerSiguienteNumeroComprobanteAsync(dto.TipoComprobante);

            _logger.LogInformation("📄 Comprobante: {Serie}-{Numero}", serie, numero);

            // 5️⃣ Construcción de items
            var detalles = venta.Detalles ?? new List<DetalleVenta>();
            if (!detalles.Any())
            {
                return BadRequest(new { ok = false, mensaje = "La venta no tiene detalles" });
            }

            var items = detalles.Select(d => new
            {
                unidad_de_medida = "NIU",
                descripcion = d.Producto?.Nombre ?? $"Producto {d.ProductoId}",
                cantidad = d.Cantidad,
                valor_unitario = Math.Round((decimal)d.Precio / 1.18m, 6), // sin IGV
                precio_unitario = d.Precio,
                subtotal = Math.Round(((decimal)d.Precio / 1.18m) * d.Cantidad, 2),
                tipo_de_igv = 1,
                igv = Math.Round((d.Precio - ((decimal)d.Precio / 1.18m)) * d.Cantidad, 2),
                total = Math.Round(d.Precio * d.Cantidad, 2)
            }).ToList();

            // 6️⃣ Totales
            decimal total = items.Sum(i => i.total);
            decimal gravada = Math.Round(total / 1.18m, 2);
            decimal igv = total - gravada;

            _logger.LogInformation("💰 Totales calculados - Total: {Total}, Gravada: {Gravada}, IGV: {IGV}",
                total, gravada, igv);

            // 7️⃣ Construir payload para Nubefact
            var payload = new
            {
                operacion = "generar_comprobante",
                tipo_de_comprobante = dto.TipoComprobante,
                serie,
                numero,
                sunat_transaction = 1,
                cliente_tipo_de_documento = dto.TipoComprobante == 1 ? 6 : 1,
                cliente_numero_de_documento = dto.TipoComprobante == 1 ? dto.RUC : dto.ClienteDNI,
                cliente_denominacion = dto.TipoComprobante == 1
                    ? dto.RazonSocial
                    : $"{dto.ClienteNombres} {dto.ClienteApellidos}",
                cliente_direccion = "LIMA",
                fecha_de_emision = DateTime.Now.ToString("yyyy-MM-dd"),
                moneda = 1,
                porcentaje_de_igv = 18.00m,
                total_gravada = gravada,
                total_igv = igv,
                total,
                items
            };

            _logger.LogInformation("🌐 Enviando payload a NubeFact: {Payload}",
                JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));

            // 8️⃣ Consumir API Nubefact
            var url = _config["NubeFact:ApiUrl"];
            var token = _config["NubeFact:Token"];

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(token))
            {
                _logger.LogError("❌ Configuración NubeFact incompleta");
                return BadRequest(new { ok = false, mensaje = "Error de configuración: Datos de NubeFact incompletos" });
            }

            var http = _httpClientFactory.CreateClient();
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("Authorization", $"Token {token}");
            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var resp = await http.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();

            _logger.LogInformation("📡 Respuesta NubeFact - Status: {StatusCode}, Body: {Body}",
                resp.StatusCode, body);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogError("❌ Error en NubeFact - Status: {StatusCode}, Response: {Response}",
                    resp.StatusCode, body);

                return BadRequest(new
                {
                    ok = false,
                    mensaje = "Error al emitir comprobante",
                    respuesta_nubefact = body,
                    status_code = (int)resp.StatusCode
                });
            }

            // 9️⃣ Procesar respuesta de Nubefact
            using var doc = JsonDocument.Parse(body);
            string? enlacePdf = doc.RootElement.TryGetProperty("enlace", out var el) ? el.GetString() : null;
            string? codigoHash = doc.RootElement.TryGetProperty("codigo_hash", out var h) ? h.GetString() : null;
            string? cadenaQr = doc.RootElement.TryGetProperty("cadena_para_codigo_qr", out var qr) ? qr.GetString() : null;

            // 🔟 Guardar comprobante en BD
            var comp = new ComprobanteVenta
            {
                VentaId = dto.VentaId,
                TipoComprobante = (byte)dto.TipoComprobante,
                Serie = serie,
                Numero = numero,
                EnlacePdf = enlacePdf,
                CodigoHash = codigoHash,
                CodigoQr = cadenaQr,
                FechaGeneracion = DateTime.Now
            };

            await _ventaService.ActualizarComprobanteVentaAsync(dto.VentaId, comp);

            _logger.LogInformation("✅ Comprobante emitido exitosamente - {Serie}-{Numero}", serie, numero);

            return Ok(new
            {
                ok = true,
                serie,
                numero,
                enlace_pdf = enlacePdf,
                codigo_hash = codigoHash,
                codigo_qr = cadenaQr,
                mensaje = $"Comprobante {serie}-{numero} emitido correctamente",
                respuesta_nubefact = JsonSerializer.Deserialize<object>(body)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error inesperado al emitir comprobante para VentaId: {VentaId}", dto.VentaId);

            return StatusCode(500, new
            {
                ok = false,
                mensaje = "Error interno del servidor",
                detalle = ex.Message
            });
        }
    }
    // Agregar al ComprobantesController para testing

    [HttpGet("test-config")]
    public IActionResult TestConfig()
    {
        var config = new
        {
            NubeFact = new
            {
                ApiUrl = _config["NubeFact:ApiUrl"],
                Environment = _config["NubeFact:Environment"],
                SerieBoleta = _config["NubeFact:SerieBoleta"],
                SerieFactura = _config["NubeFact:SerieFactura"],
                TokenConfigured = !string.IsNullOrEmpty(_config["NubeFact:Token"])
            },
            Database = new
            {
                ConnectionString = _config.GetConnectionString("DefaultConnection")?.Substring(0, 50) + "..."
            },
            MercadoPago = new
            {
                AccessTokenConfigured = !string.IsNullOrEmpty(_config["MercadoPago:AccessToken"]),
                PublicKeyConfigured = !string.IsNullOrEmpty(_config["MercadoPago:PublicKey"])
            },
            Server = new
            {
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Timestamp = DateTime.Now
            }
        };

        return Ok(new { ok = true, mensaje = "Configuración cargada correctamente", config });
    }
}