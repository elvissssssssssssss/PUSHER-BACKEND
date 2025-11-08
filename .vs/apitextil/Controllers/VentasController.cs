// Controllers/VentasController.cs
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using apitextil.Services;
using apitextil.DTOs.Orders;
using MercadoPago.Client.Preference;
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

        // Inyección de dependencias del servicio de ventas
        public VentasController(IVentaService ventaService)
        {
            _ventaService = ventaService;
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
        [HttpPost("completa")]
        public async Task<IActionResult> CrearVentaConDetalles([FromBody] CreateVentaConDetallesDto dto)
        {
            var venta = await _ventaService.CrearVentaConDetallesAsync(dto);
            // Devuelve 201 Created con la información de la venta completa creada
            return CreatedAtAction(nameof(CrearVentaConDetalles), new { id = venta.Id }, venta);
        }

        // GET: /api/Ventas
        // Obtiene todas las ventas registradas
        [HttpGet]
        public async Task<IActionResult> ObtenerVentas()
        {
            var ventas = await _ventaService.ObtenerVentasAsync();
            return Ok(ventas); // Devuelve 200 OK con la lista de ventas
        }

        // POST: /api/Ventas/preferencia
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
