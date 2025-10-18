
// Controllers/NiubizController.cs
using Microsoft.AspNetCore.Mvc;
using apitextil.Services;




namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NiubizController : ControllerBase
    {
        private readonly NiubizService _niubizService;

        public NiubizController(NiubizService niubizService)
        {
            _niubizService = niubizService;
        }

        [HttpGet("token")]
        public async Task<IActionResult> ObtenerToken()
        {
            try
            {
                var token = await _niubizService.GenerarTokenAsync();
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al generar token", error = ex.Message });
            }
        }

        [HttpPost("pago")]
        public async Task<IActionResult> ProcesarPago([FromBody] NiubizVentaRequest request)
        {
            try
            {
                var token = await _niubizService.GenerarTokenAsync();
                var respuesta = await _niubizService.EjecutarVentaAsync(request, token);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al procesar pago", error = ex.Message });
            }
        }
    }
}