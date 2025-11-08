using apitextil.Data;
using apitextil.DTOs;
using apitextil.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace apitextil.Services
{
    public class MensajeChatService : IMensajeChatService
    {
        private readonly EcommerceContext _context;
        private readonly IAtencionClienteService _atencionService;

        public MensajeChatService(EcommerceContext context, IAtencionClienteService atencionService)
        {
            _context = context;
            _atencionService = atencionService;
        }
        public async Task<HistorialChatDto?> ObtenerHistorialPorUsuarioAsync(int userId)
        {
            // Buscar la atención del usuario
            var atencion = await _context.TblAtencionCliente
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (atencion == null)
                return null;

            // Cargar los mensajes del chat
            var mensajes = await _context.TblMensajesChat
                .Where(m => m.AtencionId == atencion.Id)
                .OrderBy(m => m.CreatedAt)
                .Select(m => new MensajeChatResponseDto
                {
                    Id = m.Id,
                    AtencionId = m.AtencionId,
                    EmisorTipo = m.EmisorTipo,
                    EmisorNombre = m.EmisorNombre,
                    Mensaje = m.Mensaje,
                    Leido = m.Leido,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            // Armar la respuesta tipo DTO
            var historial = new HistorialChatDto
            {
                MensajeOriginal = new AtencionClienteResponseDto
                {
                    Id = atencion.Id,
                    UserId = atencion.UserId,
                    Nombre = atencion.Nombre,
                    Telefono = atencion.Telefono,
                    Correo = atencion.Correo,
                    Mensaje = atencion.Mensaje,
                    Estado = atencion.Estado,
                    CreatedAt = atencion.CreatedAt,
                    UpdatedAt = atencion.UpdatedAt
                },
                Mensajes = mensajes
            };

            return historial;
        }

        public async Task<MensajeChatResponseDto> GuardarMensaje(MensajeChatDto dto)
        {
            var mensaje = new TblMensajesChat
            {
                AtencionId = dto.AtencionId,
                EmisorTipo = dto.EmisorTipo,
                EmisorNombre = dto.EmisorNombre,
                Mensaje = dto.Mensaje,
                Leido = false,
                CreatedAt = DateTime.Now
            };

            _context.TblMensajesChat.Add(mensaje);
            await _context.SaveChangesAsync();

            return MapToDto(mensaje);
        }

        public async Task<List<MensajeChatResponseDto>> ObtenerHistorialPorAtencion(int atencionId)
        {
            var mensajes = await _context.TblMensajesChat
                .Where(m => m.AtencionId == atencionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            return mensajes.Select(MapToDto).ToList();
        }

        public async Task<HistorialChatDto> ObtenerHistorialCompleto(int atencionId)
        {
            var atencion = await _atencionService.ObtenerMensajePorId(atencionId);
            var mensajes = await ObtenerHistorialPorAtencion(atencionId);

            return new HistorialChatDto
            {
                MensajeOriginal = atencion,
                Mensajes = mensajes
            };
        }

        public async Task<bool> MarcarMensajesComoLeidos(int atencionId, string emisorTipo)
        {
            var mensajes = await _context.TblMensajesChat
                .Where(m => m.AtencionId == atencionId && m.EmisorTipo == emisorTipo && !m.Leido)
                .ToListAsync();

            foreach (var mensaje in mensajes)
            {
                mensaje.Leido = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private MensajeChatResponseDto MapToDto(TblMensajesChat mensaje)
        {
            return new MensajeChatResponseDto
            {
                Id = mensaje.Id,
                AtencionId = mensaje.AtencionId,
                EmisorTipo = mensaje.EmisorTipo,
                EmisorNombre = mensaje.EmisorNombre,
                Mensaje = mensaje.Mensaje,
                Leido = mensaje.Leido,
                CreatedAt = mensaje.CreatedAt
            };
        }
    }
}