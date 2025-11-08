using System.Text;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using apitextil.Models;
using apitextil.Services;
using apitextilECommerceAPI.Models;

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

        // Boleta
        public string? ClienteDNI { get; set; }
        public string? ClienteNombres { get; set; }
        public string? ClienteApellidos { get; set; }

        // Factura
        public string? RUC { get; set; }
        public string? RazonSocial { get; set; }
    }

    [HttpPost("emitir")]
    public async Task<IActionResult> Emitir([FromBody] EmitirComprobanteDto dto)
    {
        try
        {
            _logger.LogInformation("🧾 Iniciando emisión de comprobante para VentaId: {VentaId}", dto.VentaId);

            // 1) Validaciones
            if (dto.VentaId <= 0)
                return BadRequest(new { ok = false, mensaje = "VentaId inválido" });

            if (dto.TipoComprobante != 1 && dto.TipoComprobante != 2)
                return BadRequest(new { ok = false, mensaje = "Tipo de comprobante inválido. Debe ser 1 (Factura) o 2 (Boleta)" });

            if (dto.TipoComprobante == 1) // Factura
            {
                if (string.IsNullOrWhiteSpace(dto.RUC) || string.IsNullOrWhiteSpace(dto.RazonSocial))
                    return BadRequest(new { ok = false, mensaje = "Para factura se requiere RUC y Razón Social" });
            }
            else // Boleta
            {
                if (string.IsNullOrWhiteSpace(dto.ClienteNombres) || string.IsNullOrWhiteSpace(dto.ClienteApellidos))
                    return BadRequest(new { ok = false, mensaje = "Para boleta se requiere nombres y apellidos del cliente" });
            }

            // 2) Venta
            var venta = await _ventaService.ObtenerVentaCompletaAsync(dto.VentaId);
            if (venta == null)
            {
                _logger.LogWarning("❌ No se encontró la venta {VentaId}", dto.VentaId);
                return NotFound(new { ok = false, mensaje = $"No existe la venta {dto.VentaId}" });
            }
            _logger.LogInformation("✅ Venta encontrada. Detalles: {DetallesCount}", venta.Detalles?.Count ?? 0);

            // 3) Serie
            string? serie = dto.TipoComprobante == 1
                ? _config["NubeFact:SerieFactura"]
                : _config["NubeFact:SerieBoleta"];

            if (string.IsNullOrEmpty(serie))
                return BadRequest(new { ok = false, mensaje = "Error de configuración: Serie no encontrada" });

            // 4) Construcción de items / totales
            var detalles = venta.Detalles ?? new List<DetalleVenta>();
            if (!detalles.Any())
                return BadRequest(new { ok = false, mensaje = "La venta no tiene detalles" });

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

            decimal total = items.Sum(i => i.total);
            decimal gravada = Math.Round(total / 1.18m, 2);
            decimal igv = total - gravada;

            _logger.LogInformation("💰 Totales - Total: {Total}, Gravada: {Gravada}, IGV: {IGV}", total, gravada, igv);

            // 5) Payload para Nubefact (sin 'numero' salvo forzado)
            int? numeroForzado = (dto.NumeroForzado.HasValue && dto.NumeroForzado.Value > 0)
                ? dto.NumeroForzado.Value
                : (int?)null;

            var payload = new Dictionary<string, object?>
            {
                ["operacion"] = "generar_comprobante",
                ["tipo_de_comprobante"] = dto.TipoComprobante,
                ["serie"] = serie,
                ["codigo_unico"] = $"VENTA-{dto.VentaId}-{Guid.NewGuid()}", // ✅ identificador único
                ["sunat_transaction"] = 1,
                ["cliente_tipo_de_documento"] = dto.TipoComprobante == 1 ? 6 : 1,
                ["cliente_numero_de_documento"] = dto.TipoComprobante == 1 ? dto.RUC : dto.ClienteDNI,
                ["cliente_denominacion"] = dto.TipoComprobante == 1
                    ? dto.RazonSocial
                    : $"{dto.ClienteNombres} {dto.ClienteApellidos}",
                ["cliente_direccion"] = "LIMA",
                ["fecha_de_emision"] = DateTime.Now.ToString("yyyy-MM-dd"),
                ["moneda"] = 1,
                ["porcentaje_de_igv"] = 18.00m,
                ["total_gravada"] = gravada,
                ["total_igv"] = igv,
                ["total"] = total,
                ["items"] = items
            };

            if (numeroForzado.HasValue && numeroForzado.Value > 0)
            {
                payload["numero"] = numeroForzado.Value; // 👈 solo si fuerzas
                _logger.LogInformation("📄 Enviando numero forzado: {Numero}", numeroForzado.Value);
            }
            else
            {
                _logger.LogInformation("📄 Sin numero forzado: dejaremos que Nubefact asigne correlativo");
            }

            var url = _config["NubeFact:ApiUrl"];
            var token = _config["NubeFact:Token"];
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(token))
                return BadRequest(new { ok = false, mensaje = "Error de configuración: Datos de NubeFact incompletos" });

            var http = _httpClientFactory.CreateClient();
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Add("Authorization", $"Token {token}");
            var jsonPayload = JsonSerializer.Serialize(payload);
            _logger.LogInformation("🌐 Payload a NubeFact: {Payload}", jsonPayload);
            req.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // 6) Llamada a Nubefact
            var resp = await http.SendAsync(req);
            var body = await resp.Content.ReadAsStringAsync();
            _logger.LogInformation("📡 Nubefact - Status: {StatusCode}, Body: {Body}", resp.StatusCode, body);

            if (!resp.IsSuccessStatusCode)
            {
                // Intentar parsear error de Nubefact
                try
                {
                    using var errDoc = JsonDocument.Parse(body);
                    var root = errDoc.RootElement;

                    var codigo = root.TryGetProperty("codigo", out var c) ? c.GetInt32() : (int?)null;
                    var errors = root.TryGetProperty("errors", out var e) ? e.GetString() : null;

                    if (codigo == 23)
                    {
                        // Documento ya existe en Nubefact: devolver el guardado si lo tienes
                        // TODO: Ajusta el nombre del método a tu servicio
                        var existente = await _ventaService.ObtenerComprobantePorVentaAsync(dto.VentaId);
                        if (existente != null)
                        {
                            return Ok(new
                            {
                                ok = true,
                                serie = existente.Serie,
                                numero = existente.Numero,
                                enlace_pdf = existente.EnlacePdf,
                                ya_existia = true,
                                mensaje = "El comprobante ya existía en Nubefact y fue recuperado."
                            });
                        }

                        return Conflict(new
                        {
                            ok = false,
                            mensaje = "El comprobante ya existe en Nubefact (código 23) pero no está registrado en tu BD. Sincroniza o recupera el documento.",
                            respuesta_nubefact = body
                        });
                    }
                }
                catch { /* parseo falló, sigue el 400 genérico */ }

                return BadRequest(new
                {
                    ok = false,
                    mensaje = "Error al emitir comprobante",
                    respuesta_nubefact = body,
                    status_code = (int)resp.StatusCode
                });
            }

            // 7) Éxito: leer campos con nombres correctos
            using var doc = JsonDocument.Parse(body);

            string? serieResp = doc.RootElement.TryGetProperty("serie", out var se) ? se.GetString() : serie;
            int? numeroResp = doc.RootElement.TryGetProperty("numero", out var nu) ? nu.GetInt32() : null;

            string? enlacePdf =
                doc.RootElement.TryGetProperty("enlace_del_pdf", out var elPdf) ? elPdf.GetString()
              : doc.RootElement.TryGetProperty("enlace", out var elAlt) ? elAlt.GetString()
              : null;

            string? codigoHash = doc.RootElement.TryGetProperty("codigo_hash", out var h) ? h.GetString() : null;
            string? cadenaQr = doc.RootElement.TryGetProperty("cadena_para_codigo_qr", out var qr) ? qr.GetString() : null;

            // 8) Guardar en BD (usar serie/numero devueltos por Nubefact si existen)
            var comp = new ComprobanteVenta
            {
                VentaId = dto.VentaId,
                TipoComprobante = (byte)dto.TipoComprobante,
                Serie = serieResp ?? serie ?? "",
                Numero = numeroResp ?? 0,
                EnlacePdf = enlacePdf,
                CodigoHash = codigoHash,
                CodigoQr = cadenaQr,
                FechaGeneracion = DateTime.Now
            };

            await _ventaService.ActualizarComprobanteVentaAsync(dto.VentaId, comp);
            _logger.LogInformation("✅ Comprobante emitido - {Serie}-{Numero}", comp.Serie, comp.Numero);

            return Ok(new
            {
                ok = true,
                serie = comp.Serie,
                numero = comp.Numero,
                enlace_pdf = comp.EnlacePdf,
                codigo_hash = comp.CodigoHash,
                codigo_qr = comp.CodigoQr,
                mensaje = $"Comprobante {comp.Serie}-{comp.Numero} emitido correctamente",
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

    // Utilidad para verificar configuración en Render
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
