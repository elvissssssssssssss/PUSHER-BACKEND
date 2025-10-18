
// Services/TblEnviosService.cs
using Microsoft.EntityFrameworkCore;
using apitextil.Data;
using apitextil.DTOs;
using apitextil.Models;

namespace apitextil.Services
{
    public class TblEnviosService : ITblEnviosService
    {
        private readonly EcommerceContext _context;

        public TblEnviosService(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<TblEnviosResponseDto> CreateAsync(TblEnviosCreateDto dto)
        {
            // 🔹 Se eliminan las validaciones de duplicado para permitir repetición
            // antes: ExistsByUserIdAsync y ExistsByDniAsync

            var envio = new TblEnvios
            {
                UserId = dto.UserId,
                Direccion = dto.Direccion,
                Region = dto.Region,
                Provincia = dto.Provincia,
                Localidad = dto.Localidad,
                Dni = dto.Dni,
                Telefono = dto.Telefono,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TblEnvios.Add(envio);
            await _context.SaveChangesAsync();

            return MapToResponseDto(envio);
        }

        public async Task<TblEnviosResponseDto?> GetByIdAsync(int id)
        {
            var envio = await _context.TblEnvios
                .FirstOrDefaultAsync(e => e.Id == id);

            return envio != null ? MapToResponseDto(envio) : null;
        }

        public async Task<TblEnviosResponseDto?> GetByUserIdAsync(int userId)
        {
            var envio = await _context.TblEnvios
                .FirstOrDefaultAsync(e => e.UserId == userId);

            return envio != null ? MapToResponseDto(envio) : null;
        }

        public async Task<IEnumerable<TblEnviosResponseDto>> GetAllAsync()
        {
            var envios = await _context.TblEnvios
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return envios.Select(MapToResponseDto);
        }

        public async Task<TblEnviosResponseDto?> UpdateAsync(int id, TblEnviosUpdateDto dto)
        {
            var envio = await _context.TblEnvios
                .FirstOrDefaultAsync(e => e.Id == id);

            if (envio == null)
                return null;

            // 🔹 Se quita validación de DNI duplicado
            if (!string.IsNullOrEmpty(dto.Direccion))
                envio.Direccion = dto.Direccion;
            if (!string.IsNullOrEmpty(dto.Region))
                envio.Region = dto.Region;
            if (!string.IsNullOrEmpty(dto.Provincia))
                envio.Provincia = dto.Provincia;
            if (!string.IsNullOrEmpty(dto.Localidad))
                envio.Localidad = dto.Localidad;
            if (!string.IsNullOrEmpty(dto.Dni))
                envio.Dni = dto.Dni;
            if (!string.IsNullOrEmpty(dto.Telefono))
                envio.Telefono = dto.Telefono;

            envio.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToResponseDto(envio);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var envio = await _context.TblEnvios
                .FirstOrDefaultAsync(e => e.Id == id);

            if (envio == null)
                return false;

            _context.TblEnvios.Remove(envio);
            await _context.SaveChangesAsync();
            return true;
        }

        // 🔹 Ya no se usan, pero las dejo por si en el futuro quieres validar
        public async Task<bool> ExistsByDniAsync(string dni)
        {
            return await _context.TblEnvios
                .AnyAsync(e => e.Dni == dni);
        }

        public async Task<bool> ExistsByUserIdAsync(int userId)
        {
            return await _context.TblEnvios
                .AnyAsync(e => e.UserId == userId);
        }

        public async Task<IEnumerable<TblEnviosResponseDto>> GetByRegionAsync(string region)
        {
            var envios = await _context.TblEnvios
                .Where(e => e.Region.ToLower() == region.ToLower())
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return envios.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<TblEnviosResponseDto>> GetByProvinciaAsync(string provincia)
        {
            var envios = await _context.TblEnvios
                .Where(e => e.Provincia.ToLower() == provincia.ToLower())
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return envios.Select(MapToResponseDto);
        }

        private static TblEnviosResponseDto MapToResponseDto(TblEnvios envio)
        {
            return new TblEnviosResponseDto
            {
                Id = envio.Id,
                UserId = envio.UserId,
                Direccion = envio.Direccion,
                Region = envio.Region,
                Provincia = envio.Provincia,
                Localidad = envio.Localidad,
                Dni = envio.Dni,
                Telefono = envio.Telefono,
                CreatedAt = envio.CreatedAt,
                UpdatedAt = envio.UpdatedAt
            };
        }
    }
}
