
//ChatController.cs

using Microsoft.AspNetCore.Mvc;
using PusherServer;
using apitextil.DTOs;
using apitextil.Services;

using System;
using System.Threading.Tasks;

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IAtencionClienteService _atencionService;
       
        private readonly IMensajeChatService _mensajeChatService;

        public ChatController(
         IAtencionClienteService atencionService,
         IMensajeChatService mensajeChatService)
        {
            _atencionService = atencionService;
            _mensajeChatService = mensajeChatService;
        }
        // POST: api/Chat/contacto
        [HttpPost("contacto")]
        public async Task<IActionResult> EnviarContacto([FromBody] AtencionClienteDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Guardar en la base de datos
                var resultado = await _atencionService.CrearMensajeContacto(dto);

                return Ok(new
                {
                    success = true,
                    mensaje = "Mensaje enviado correctamente",
                    data = resultado
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ex.Message
                });
            }
        }

        // GET: api/Chat/mensajes
        [HttpGet("mensajes")]
        public async Task<IActionResult> ObtenerMensajes()
        {
            try
            {
                var mensajes = await _atencionService.ObtenerTodosMensajes();
                return Ok(new { success = true, data = mensajes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        // GET: api/Chat/mensajes/5
        [HttpGet("mensajes/{id}")]
        public async Task<IActionResult> ObtenerMensajePorId(int id)
        {
            try
            {
                var mensaje = await _atencionService.ObtenerMensajePorId(id);
                return Ok(new { success = true, data = mensaje });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        // GET: api/Chat/mensajes/estado/pendiente
        [HttpGet("mensajes/estado/{estado}")]
        public async Task<IActionResult> ObtenerMensajesPorEstado(string estado)
        {
            try
            {
                var mensajes = await _atencionService.ObtenerMensajesPorEstado(estado);
                return Ok(new { success = true, data = mensajes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        // POST: api/Chat/guardar-mensaje
        [HttpPost("guardar-mensaje")]
        public async Task<IActionResult> GuardarMensajeChat([FromBody] MensajeChatDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var resultado = await _mensajeChatService.GuardarMensaje(dto);

                return Ok(new
                {
                    success = true,
                    mensaje = "Mensaje guardado correctamente",
                    data = resultado
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        // GET: api/Chat/historial/{atencionId}
        [HttpGet("historial/{atencionId}")]
        public async Task<IActionResult> ObtenerHistorial(int atencionId)
        {
            try
            {
                var historial = await _mensajeChatService.ObtenerHistorialCompleto(atencionId);
                return Ok(new { success = true, data = historial });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        // PUT: api/Chat/marcar-leidos/{atencionId}/{emisorTipo}
        [HttpPut("marcar-leidos/{atencionId}/{emisorTipo}")]
        public async Task<IActionResult> MarcarComoLeidos(int atencionId, string emisorTipo)
        {
            try
            {
                var resultado = await _mensajeChatService.MarcarMensajesComoLeidos(atencionId, emisorTipo);
                return Ok(new { success = true, mensaje = "Mensajes marcados como leídos" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        // PUT: api/Chat/mensajes/5/estado
        [HttpPut("mensajes/{id}/estado")]
        public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoDto dto)
        {
            try
            {
                var resultado = await _atencionService.ActualizarEstado(id, dto.Estado);

                if (!resultado)
                    return NotFound(new { success = false, error = "Mensaje no encontrado" });

                return Ok(new { success = true, mensaje = "Estado actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
        // POST: api/Chat/enviar-y-guardar
        [HttpPost("enviar-y-guardar")]
        public async Task<IActionResult> EnviarYGuardar([FromBody] EnviarYGuardarDto dto)
        {
            try
            {
                // 1. Guardar en la base de datos
                var mensajeDto = new MensajeChatDto
                {
                    AtencionId = dto.AtencionId,
                    EmisorTipo = dto.Tipo,
                    EmisorNombre = dto.Usuario,
                    Mensaje = dto.Texto
                };

                var resultado = await _mensajeChatService.GuardarMensaje(mensajeDto);

                // 2. Enviar por Pusher
                var options = new PusherOptions
                {
                    Cluster = "mt1",
                    Encrypted = true
                };
                var pusher = new Pusher("2062327", "058be5b82a25fa9d45d6", "8c176f166837db10cf76", options);

                await pusher.TriggerAsync("chat-soporte", "nuevo-mensaje", new
                {
                    atencionId = dto.AtencionId,
                    usuario = dto.Usuario,
                    texto = dto.Texto,
                    tipo = dto.Tipo,
                    fecha = DateTime.Now
                });

                return Ok(new
                {
                    success = true,
                    mensaje = "Mensaje enviado y guardado",
                    data = resultado
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    
    // DTO auxiliar
    public class EnviarYGuardarDto
    {
        public int AtencionId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }

        // GET: api/Chat/historial/usuario/{userId}
        // GET: api/Chat/historial/usuario/{userId}
        [HttpGet("historial/usuario/{userId}")]
        public async Task<IActionResult> ObtenerHistorialPorUsuario(int userId)
        {
            var historial = await _mensajeChatService.ObtenerHistorialPorUsuarioAsync(userId);

            if (historial == null)
                return NotFound(new { success = false, message = "No se encontró historial para este usuario" });

            return Ok(new { success = true, data = historial });
        }


        // POST: api/Chat/enviar (tu endpoint de Pusher existente)
        [HttpPost("enviar")]
        public async Task<IActionResult> Enviar([FromBody] ChatMessageDto mensaje)
        {
            try
            {
                var options = new PusherOptions
                {
                    Cluster = "mt1",
                    Encrypted = true
                };
                var pusher = new Pusher(
                    "2062327",
                    "058be5b82a25fa9d45d6",
                    "8c176f166837db10cf76",
                    options
                );

                await pusher.TriggerAsync("chat-soporte", "nuevo-mensaje", new
                {
                    usuario = mensaje.Usuario,
                    texto = mensaje.Texto,
                    tipo = mensaje.Tipo,
                    fecha = mensaje.Fecha
                });

                return Ok(new { success = true, mensaje = "Mensaje enviado a Pusher correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }
    }


    // DTO auxiliar para actualizar estado
    public class ActualizarEstadoDto
    {
        public string Estado { get; set; } = string.Empty;
    }
}