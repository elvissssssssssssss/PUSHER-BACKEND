// Controllers/VentasController.cs
using apitextil.Data;
using apitextil.DTOs.Orders;
using apitextil.Services;
using apitextil.Services;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _ventaService;
        private readonly IEmailService _emailService;
        private readonly EcommerceContext _context; // ✅ Solo esta línea

        // ✅ Constructor correcto con 3 parámetros
        public VentasController(
            IVentaService ventaService,
            IEmailService emailService,
            EcommerceContext context)  // ✅ Agregar context aquí
        {
            _ventaService = ventaService;
            _emailService = emailService;
            _context = context;  // ✅ Ahora sí funciona
        }


        // POST: /api/Ventas
        // Crea una venta simple sin detalles adicionales
        [HttpPost]
        public async Task<IActionResult> CrearVenta([FromBody] CreateVentaDto dto)
        {
            var venta = await _ventaService.CrearVentaAsync(dto);
            // Devuelve 201 Created con la información de la venta creada
            return CreatedAtAction(nameof(CrearVenta), new { id = venta.Id }, venta);
        }

        // POST: /api/Ventas/completa
        // Crea una venta junto con sus detalles (productos, cantidades, etc.)
        // POST: /api/Ventas/completa
        [HttpPost("completa")]
        public async Task<IActionResult> CrearVentaConDetalles([FromBody] CreateVentaConDetallesDto dto)
        {
            try
            {
                Console.WriteLine("🛒 === INICIO CrearVentaConDetalles ===");
                Console.WriteLine($"UserId: {dto.UserId}, Total: {dto.Total}");

                // Crear venta y obtener DTO con datos del usuario
                var ventaDto = await _ventaService.CrearVentaConDetallesAsync(dto);

                Console.WriteLine($"✅ Venta creada - ID: {ventaDto.Id}");
                Console.WriteLine($"📧 Email: {ventaDto.UsuarioEmail}");
                Console.WriteLine($"👤 Nombre: {ventaDto.UsuarioNombre}");
                Console.WriteLine($"💰 Total: {ventaDto.Total}");

                // ❌ COMENTAR ESTA SECCIÓN COMPLETA (no enviar email aquí)
                /*
                if (!string.IsNullOrEmpty(ventaDto.UsuarioEmail) &&
                    !string.IsNullOrEmpty(ventaDto.UsuarioNombre))
                {
                    try
                    {
                        Console.WriteLine($"📨 INTENTANDO enviar email a: {ventaDto.UsuarioEmail}");

                        await _emailService.EnviarEmailVentaExitosaAsync(
                            ventaDto.UsuarioEmail,
                            ventaDto.UsuarioNombre,
                            ventaDto.Total,
                            ventaDto.Id.ToString()
                        );

                        Console.WriteLine($"✅✅✅ EMAIL ENVIADO EXITOSAMENTE a: {ventaDto.UsuarioEmail}");
                    }
                    catch (Exception emailEx)
                    {
                        Console.WriteLine($"❌❌❌ ERROR al enviar email: {emailEx.Message}");
                        Console.WriteLine($"Stack trace: {emailEx.StackTrace}");
                        Console.WriteLine($"Inner exception: {emailEx.InnerException?.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("⚠️ NO se enviará email - Email o Nombre vacío");
                }
                */

                Console.WriteLine("🛒 === FIN CrearVentaConDetalles ===");
                return CreatedAtAction(nameof(CrearVentaConDetalles), new { id = ventaDto.Id }, ventaDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR GENERAL en CrearVentaConDetalles: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }


        // GET: /api/Ventas
        // Obtiene todas las ventas registradas
        [HttpGet]
        public async Task<IActionResult> ObtenerVentas()
        {
            var ventas = await _ventaService.ObtenerVentasAsync();
            return Ok(ventas); // Devuelve 200 OK con la lista de ventas
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailDto dto)
        {
            try
            {
                await _emailService.EnviarEmailVentaExitosaAsync(
                    dto.Email,
                    "Cliente de Prueba",
                    100.50m,
                    "TEST-001"
                );
                return Ok(new { message = "Email enviado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DTO para pruebas
        public class TestEmailDto
        {
            public string Email { get; set; }
        }
        // POST: /api/Ventas/notificar-comprobante
        [HttpPost("notificar-comprobante")]
        public async Task<IActionResult> NotificarComprobante([FromBody] NotificarComprobanteDto dto)
        {
            try
            {
                Console.WriteLine($"📧 === Notificando comprobante para venta {dto.VentaId} ===");

                var venta = await _context.Ventas
                    .Include(v => v.User)
                    .FirstOrDefaultAsync(v => v.Id == dto.VentaId);

                if (venta == null)
                {
                    return NotFound(new { error = $"Venta {dto.VentaId} no encontrada" });
                }

                var comprobante = await _ventaService.ObtenerComprobantePorVentaAsync(dto.VentaId);

                if (comprobante == null || string.IsNullOrEmpty(comprobante.EnlacePdf))
                {
                    return BadRequest(new { error = "No se encontró comprobante con PDF" });
                }

                var nombreCompleto = $"{venta.User.nombre} {venta.User.apellido}";

                await _emailService.EnviarEmailVentaExitosaAsync(
                    venta.User.email,
                    nombreCompleto,
                    venta.Total,
                    venta.Id.ToString(),
                    comprobante.EnlacePdf
                );

                Console.WriteLine($"✅ Email con PDF enviado a {venta.User.email}");

                return Ok(new { success = true, message = "Email enviado con comprobante" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // DTO para el endpoint
        public class NotificarComprobanteDto
        {
            public int VentaId { get; set; }
        }

        // Crea una preferencia de pago en Mercado Pago para iniciar el proceso de pago
        [HttpPost("preferencia")]
        public async Task<IActionResult> CrearPreferenciaPago([FromBody] CreatePreferenceRequestDto dto)
        {
            try
            {
                // Inicializar Mercado Pago con tu AccessToken
                MercadoPagoConfig.AccessToken = "APP_USR-6319008843131615-081423-64feacca70c1b93d52ea849961aa46a4-2623525121";
                // ⚠ IMPORTANTE: No dejar el token visible en código, usar configuración segura (appsettings o variables de entorno)

                // Crear el objeto de preferencia para Mercado Pago
                var request = new PreferenceRequest
                {
                    Items = new List<PreferenceItemRequest>()
                };

                // Recorrer los ítems enviados desde el cliente y agregarlos a la preferencia
                foreach (var item in dto.Items)
                {
                    request.Items.Add(new PreferenceItemRequest
                    {
                        Title = item.Title,       // Nombre del producto
                        Quantity = item.Quantity, // Cantidad
                        CurrencyId = "PEN",       // Moneda: Soles peruanos
                        UnitPrice = item.UnitPrice // Precio unitario
                    });
                }

                // Crear la preferencia de pago usando el cliente de Mercado Pago
                var client = new PreferenceClient();
                Preference preference = await client.CreateAsync(request);

                // Devolver los datos necesarios para iniciar el pago
                return Ok(new
                {
                    PreferenceId = preference.Id,
                    InitPoint = preference.InitPoint, // URL para iniciar pago en producción
                    SandboxInitPoint = preference.SandboxInitPoint // URL para pruebas
                });
            }
            catch (Exception ex)
            {
                // Si ocurre un error, devolver 400 Bad Request con el mensaje
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
