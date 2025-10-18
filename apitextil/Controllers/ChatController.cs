


using Microsoft.AspNetCore.Mvc;
using PusherServer;
using apitextil.DTOs;
using apitextil.Models.DTOs;
using System;


namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
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
                    "2062327", // App ID
                    "058be5b82a25fa9d45d6", // Key
                    "8c176f166837db10cf76", // Secret
                    options
                );

                // Envía el mensaje a todos los clientes suscritos al canal "chat-soporte"
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
}