using apitextil.Data; // tu DbContext
using apitextil.DTOs;
using apitextil.Services;
using apitextil.Services.apitextil.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PusherServer;

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class EnviosController : ControllerBase
    {
        private readonly IEnvioService _envioService;
        private readonly EcommerceContext _context;
        private readonly IEnvioNotificacionService _notificacionService; // ⬅️ NUEVO


        public EnviosController(IEnvioService envioService, EcommerceContext context,
             IEnvioNotificacionService notificacionService) // ⬅️ NUEVO)
        {
            _envioService = envioService;
            _context = context;
            _notificacionService = notificacionService; // ⬅️ NUEVO                  
        }


        [HttpGet("estados")]
        public async Task<IActionResult> GetEstadosEnvio()
        {
            try
            {
                var estados = await _envioService.GetEstadosEnvioAsync();
                return Ok(estados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener estados: {ex.Message}" });
            }
        }

        [HttpGet("seguimiento/{ventaId}")]
        public async Task<IActionResult> GetSeguimientoByVentaId(int ventaId)
        {
            try
            {
                var seguimiento = await _envioService.GetSeguimientoByVentaIdAsync(ventaId);
                return Ok(seguimiento);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener seguimiento: {ex.Message}" });
            }
        }

        [HttpPost("seguimiento")]
        public async Task<IActionResult> AddSeguimiento([FromBody] CreateSeguimientoEnvioDto seguimientoDto)
        {
            try
            {
                // 1. Agregar el seguimiento
                var result = await _envioService.AddSeguimientoAsync(seguimientoDto);

                if (!result)
                    return BadRequest(new { message = "Error al agregar seguimiento" });

                // 2. ENVIAR NOTIFICACIÓN PUSH ⬅️ NUEVO
                await _notificacionService.NotificarActualizacionEnvio(
                    seguimientoDto.venta_id,
                    seguimientoDto.estado_id,
                    seguimientoDto.ubicacion_actual,
                    seguimientoDto.observaciones
                );

                return Ok(new { message = "Seguimiento agregado y notificación enviada" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al agregar seguimiento: {ex.Message}" });
            }
        }

        [HttpGet("documentos/{ventaId}")]
        public async Task<IActionResult> GetDocumentosByVentaId(int ventaId)
        {
            try
            {
                var documentos = await _envioService.GetDocumentosByVentaIdAsync(ventaId);
                return Ok(documentos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al obtener documentos: {ex.Message}" });
            }
        }

        [HttpPost("documentos")]
        public async Task<IActionResult> AddDocumento([FromBody] CreateDocumentoEnvioDto documentoDto)
        {
            try
            {
                var result = await _envioService.AddDocumentoAsync(documentoDto);
                if (result)
                    return Ok(new { message = "Documento agregado correctamente" });
                else
                    return BadRequest(new { message = "Error al agregar documento" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al agregar documento: {ex.Message}" });
            }
        }

        [HttpPost("upload-documento")]
        public async Task<IActionResult> UploadDocumento([FromForm] UploadDocumentoDto uploadDto)
        {
            try
            {
                var rutaArchivo = await _envioService.UploadDocumentoAsync(uploadDto.archivo, uploadDto.tipo_documento);

                var documentoDto = new CreateDocumentoEnvioDto
                {
                    venta_id = uploadDto.venta_id,
                    tipo_documento = uploadDto.tipo_documento,
                    nombre_archivo = uploadDto.archivo.FileName,
                    ruta_archivo = rutaArchivo
                };

                var result = await _envioService.AddDocumentoAsync(documentoDto);

                if (result)
                    return Ok(new { message = "Documento subido correctamente", ruta = rutaArchivo });
                else
                    return BadRequest(new { message = "Error al guardar documento en base de datos" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al subir documento: {ex.Message}" });
            }
        }

        [HttpPost("confirmar-entrega/{ventaId}")]
        public async Task<IActionResult> ConfirmarEntrega(int ventaId)
        {
            try
            {
                var result = await _envioService.ConfirmarEntregaAsync(ventaId);

                return result
                    ? Ok(new { message = "Entrega confirmada correctamente" })
                    : BadRequest(new { message = "No se pudo confirmar la entrega" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error al confirmar entrega: {ex.Message}" });
            }
        }

        [HttpGet("mis-seguimientos")]
        public async Task<IActionResult> GetMisSeguimientos()
        {
            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var envios = await _context.TblEnvios
                .Where(e => e.UserId == userId)
                .ToListAsync();

            return Ok(envios);
        }

        // ⬇️ NUEVO: Endpoint para autenticación de Pusher (necesario para canales privados)
        [HttpPost("pusher/auth")]
        [AllowAnonymous] // ⬅️ PERMITE ACCESO SIN TOKEN
        public IActionResult AuthPusher([FromBody] PusherAuthRequest request)
        {
            Console.WriteLine("🔐 =================================");
            Console.WriteLine($"🔐 AuthPusher llamado (sin autenticación)");
            Console.WriteLine($"📡 Channel: {request?.channel_name ?? "NULL"}");
            Console.WriteLine($"🆔 Socket ID: {request?.socket_id ?? "NULL"}");

            // Extraer userId del nombre del canal
            // Formato esperado: "private-user-28"
            var channelParts = request.channel_name.Split('-');
            if (channelParts.Length != 3 || channelParts[0] != "private" || channelParts[1] != "user")
            {
                Console.WriteLine("❌ Formato de canal inválido");
                return BadRequest(new { error = "Formato de canal inválido" });
            }

            if (!int.TryParse(channelParts[2], out int userId))
            {
                Console.WriteLine("❌ User ID inválido en el canal");
                return BadRequest(new { error = "User ID inválido" });
            }

            Console.WriteLine($"✅ User ID extraído del canal: {userId}");

            var pusherOptions = new PusherOptions
            {
                Cluster = "mt1",
                Encrypted = true
            };

            var pusher = new Pusher(
                "2062327",
                "058be5b82a25fa9d45d6",
                "8c176f166837db10cf76",
                pusherOptions
            );

            var auth = pusher.Authenticate(request.channel_name, request.socket_id);
            Console.WriteLine($"✅ Autenticación Pusher exitosa para usuario {userId}");
            Console.WriteLine("🔐 =================================");

            return Ok(auth.ToJson());
        }


    }

    // ⬇️ NUEVO: Clase para la solicitud de autenticación
    public class PusherAuthRequest
    {
        public string socket_id { get; set; }
        public string channel_name { get; set; }
    }
}