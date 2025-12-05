// Controllers/VentasController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using apitextil.Services;
using apitextil.DTOs.Orders;
using MercadoPago.Client.Preference;
using apitextil.Services;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;


namespace apitextil.Controllers
{
    [ApiController] // Indica que este controlador manejará peticiones HTTP de tipo API
    [Route("api/[controller]")] // La ruta base será "api/Ventas"
    public class VentasController : ControllerBase
    {
        // Servicio que maneja la lógica de negocio para ventas
        private readonly IVentaService _ventaService;
        private readonly IEmailService _emailService; // Agregar

        // Inyección de dependencias del servicio de ventas
        public VentasController(IVentaService ventaService, IEmailService emailService)
        {
            _ventaService = ventaService;
            _emailService = emailService;
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
                // Crear venta y obtener DTO con datos del usuario
                var ventaDto = await _ventaService.CrearVentaConDetallesAsync(dto);

                // ✨ Enviar email usando los campos del DTO
                if (!string.IsNullOrEmpty(ventaDto.UsuarioEmail) &&
                    !string.IsNullOrEmpty(ventaDto.UsuarioNombre))
                {
                    try
                    {
                        await _emailService.EnviarEmailVentaExitosaAsync(
                            ventaDto.UsuarioEmail,      // Email de tblusuarios
                            ventaDto.UsuarioNombre,     // Nombre completo
                            ventaDto.Total,             // Total de la venta
                            ventaDto.Id.ToString()      // ID de la venta
                        );
                    }
                    catch (Exception emailEx)
                    {
                        // Log del error pero no falla la venta
                        Console.WriteLine($"Error al enviar email: {emailEx.Message}");
                    }
                }

                return CreatedAtAction(nameof(CrearVentaConDetalles), new { id = ventaDto.Id }, ventaDto);
            }
            catch (Exception ex)
            {
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
