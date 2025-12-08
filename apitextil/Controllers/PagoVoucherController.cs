using apitextil.Data;
using apitextil.DTOs;
using apitextil.Models;
using apitextilECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace apitextil.Controllers
{
    [Route("api/Ventas/pago")]
    [ApiController]
    public class PagoVoucherController : ControllerBase
    {
        private readonly EcommerceContext _context;
        private readonly IWebHostEnvironment _env;

        public PagoVoucherController(EcommerceContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST: api/Ventas/pago/voucher-completo
        [HttpPost("voucher-completo")]
        public async Task<IActionResult> SubirVoucherCompleto([FromForm] VoucherRequestDto request)
        {
            Console.WriteLine(">>> Entró a SubirVoucherCompleto");

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState)
                {
                    var field = kvp.Key;
                    var state = kvp.Value;
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"❌ ModelState error en '{field}': {error.ErrorMessage}");
                    }
                }

                return BadRequest(new
                {
                    success = false,
                    message = "Errores de validación en el modelo",
                    errors = ModelState
                });
            }

            // ✅ USAR LA ESTRATEGIA DE EJECUCIÓN CORRECTA
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    Console.WriteLine("📥 Iniciando procesamiento de voucher...");

                    // 1. Validar voucher
                    if (request.Voucher == null || request.Voucher.Length == 0)
                    {
                        return BadRequest(new ErrorResponseDto
                        {
                            Success = false,
                            Message = "Debe subir el comprobante de pago"
                        });
                    }

                    // Validar tamaño (máximo 5MB)
                    if (request.Voucher.Length > 5 * 1024 * 1024)
                    {
                        return BadRequest(new ErrorResponseDto
                        {
                            Success = false,
                            Message = "El archivo no debe superar los 5MB"
                        });
                    }

                    // 2. Guardar archivo del voucher
                    string voucherFileName = await GuardarVoucherAsync(request.Voucher);
                    Console.WriteLine($"✅ Voucher guardado: {voucherFileName}");

                    // 3. Crear venta en tblventa
                    var venta = new Venta
                    {
                        UserId = request.UserId,
                        MetodoPagoId = 1, // ID método de pago voucher/yape
                        Total = request.Total,
                        FechaVenta = DateTime.Now,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Ventas.Add(venta);
                    await _context.SaveChangesAsync();
                    Console.WriteLine($"✅ Venta creada: {venta.Id}");

                    // 4. Insertar detalles de venta (OPCIÓN ROBUSTA)
                    Console.WriteLine("🔍 RAW Detalles recibido: " + request.Detalles);

                    List<DetalleVentaDto> detalles;

                    try
                    {
                        // Intento 1: asumir que viene como JSON válido
                        detalles = JsonSerializer.Deserialize<List<DetalleVentaDto>>(request.Detalles);
                    }
                    catch (JsonException)
                    {
                        // Intento 2: formato plano tipo:
                        // "ProductoId121,NombreProductoCASACA...,Cantidad1,PrecioUnitario123"
                        detalles = ParsearDetallesPlano(request.Detalles);
                    }

                    foreach (var detalle in detalles)
                    {
                        var detalleVenta = new DetalleVenta
                        {
                            VentaId = venta.Id,
                            ProductoId = detalle.ProductoId,
                            Cantidad = detalle.Cantidad,
                            Precio = detalle.PrecioUnitario
                        };
                        _context.DetalleVentas.Add(detalleVenta);
                    }
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Detalles de venta insertados");

                    // 5. Registrar voucher en tblvouchers
                    var voucher = new TblVoucher
                    {
                        OrderId = venta.Id, // Usamos el ID de la venta
                        VoucherArchivo = voucherFileName,
                        NumeroOperacion = request.NumeroOperacion,
                        Estado = "pendiente",
                        FechaSubida = DateTime.Now
                    };

                    _context.TblVouchers.Add(voucher);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Voucher registrado en BD");

                    // 6. Obtener email del usuario
                    var usuario = await _context.tblusuarios
                        .FirstOrDefaultAsync(u => u.id == request.UserId);

                    await transaction.CommitAsync();

                    Console.WriteLine("✅ Transacción completada exitosamente");

                    return Ok(new VoucherResponseDto
                    {
                        Success = true,
                        Message = "Voucher y venta registrados exitosamente",
                        VentaId = venta.Id,
                        OrderId = venta.Id,
                        UsuarioEmail = usuario?.email
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"❌ Error: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                    return StatusCode(500, new ErrorResponseDto
                    {
                        Success = false,
                        Message = "Error al procesar el voucher",
                        Error = ex.Message
                    });
                }
            });
        }

        // Método auxiliar para guardar el archivo
        private async Task<string> GuardarVoucherAsync(IFormFile file)
        {
            // Crear carpeta si no existe
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "vouchers");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar nombre único
            string extension = Path.GetExtension(file.FileName);
            string fileName = $"voucher-{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid()}{extension}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }

        // Parser para formato plano de Detalles
        private List<DetalleVentaDto> ParsearDetallesPlano(string input)
{
    var result = new List<DetalleVentaDto>();
    if (string.IsNullOrWhiteSpace(input))
        return result;

    var partes = input.Split(',', StringSplitOptions.RemoveEmptyEntries);
    var dto = new DetalleVentaDto();

    foreach (var parte in partes)
    {
        if (parte.StartsWith("ProductoId", StringComparison.OrdinalIgnoreCase))
        {
            var valor = parte.Replace("ProductoId", "");
            if (int.TryParse(valor, out var id))
            {
                dto.ProductoId = id;
            }
        }
        else if (parte.StartsWith("NombreProducto", StringComparison.OrdinalIgnoreCase))
        {
            dto.NombreProducto = parte.Replace("NombreProducto", "");
        }
        else if (parte.StartsWith("Cantidad", StringComparison.OrdinalIgnoreCase))
        {
            var valor = parte.Replace("Cantidad", "");
            if (int.TryParse(valor, out var cant))
            {
                dto.Cantidad = cant;
            }
        }
        else if (parte.StartsWith("PrecioUnitario", StringComparison.OrdinalIgnoreCase))
        {
            var valor = parte.Replace("PrecioUnitario", "");
            if (decimal.TryParse(valor, out var precio))
            {
                dto.PrecioUnitario = precio;
            }
        }
    }

    result.Add(dto);
    return result;
}
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("voucher-completo-test")]
        public IActionResult Test([FromForm] IFormFile voucher)
        {
            Console.WriteLine("✅ Entró al action Test");
            return Ok();
        }
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            Console.WriteLine("🏓 Entró a /api/Ventas/pago/ping");
            return Ok("pong");
        }

        // GET: api/Ventas/pago/config
        [HttpGet("config")]
        public async Task<IActionResult> ObtenerConfigPago()
        {
            try
            {
                var config = await _context.TblConfigPagos
                    .FirstOrDefaultAsync();

                if (config == null)
                {
                    return NotFound(new ErrorResponseDto
                    {
                        Success = false,
                        Message = "Configuración no encontrada"
                    });
                }

                return Ok(new
                {
                    success = true,
                    data = new ConfigPagoDto
                    {
                        CuentaSoles = config.CuentaSoles,
                        Cci = config.Cci,
                        CuentaActiva = config.CuentaActiva,
                        YapeQr = config.YapeQr
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto
                {
                    Success = false,
                    Message = "Error al obtener configuración",
                    Error = ex.Message
                });
            }
        }
    }
}
