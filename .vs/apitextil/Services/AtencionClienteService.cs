
// ============================================
// 4. Services/AtencionClienteService.cs
// ============================================
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
    public class AtencionClienteService : IAtencionClienteService
    {
        private readonly EcommerceContext _context;

        public AtencionClienteService(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<AtencionClienteResponseDto> CrearMensajeContacto(AtencionClienteDto dto)
        {
            var atencion = new TblAtencionCliente
            {
                UserId = dto.UserId,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Correo = dto.Correo,
                Mensaje = dto.Mensaje,
                Estado = "pendiente",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.TblAtencionCliente.Add(atencion);
            await _context.SaveChangesAsync();

            return MapToDto(atencion);
        }

        public async Task<List<AtencionClienteResponseDto>> ObtenerTodosMensajes()
        {
            var mensajes = await _context.TblAtencionCliente
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return mensajes.Select(MapToDto).ToList();
        }

        public async Task<AtencionClienteResponseDto> ObtenerMensajePorId(int id)
        {
            var mensaje = await _context.TblAtencionCliente.FindAsync(id);

            if (mensaje == null)
                throw new KeyNotFoundException($"Mensaje con ID {id} no encontrado");

            return MapToDto(mensaje);
        }

        public async Task<bool> ActualizarEstado(int id, string nuevoEstado)
        {
            var mensaje = await _context.TblAtencionCliente.FindAsync(id);

            if (mensaje == null)
                return false;

            mensaje.Estado = nuevoEstado;
            mensaje.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<AtencionClienteResponseDto>> ObtenerMensajesPorEstado(string estado)
        {
            var mensajes = await _context.TblAtencionCliente
                .Where(m => m.Estado == estado)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return mensajes.Select(MapToDto).ToList();
        }

        private AtencionClienteResponseDto MapToDto(TblAtencionCliente atencion)
        {
            return new AtencionClienteResponseDto
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
            };
        }
    }
}