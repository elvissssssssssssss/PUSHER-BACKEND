using apitextil.Data;
using apitextil.DTOs;
using apitextil.Models;
using Microsoft.EntityFrameworkCore;

namespace apitextil.Services
{
    public class RoleService : iRoleService
    {
        private readonly EcommerceContext _context;

        public RoleService(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Set<Rol>().OrderBy(r => r.Nombre).ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            return await _context.Set<Rol>().FindAsync(id);
        }

        public async Task<Rol> CreateAsync(CreateRolDto dto, int creadoPor)
        {
            var rol = new Rol
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                CreadoPor = creadoPor
            };
            _context.Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        public async Task<bool> UpdateAsync(int id, UpdateRolDto dto)
        {
            var rol = await _context.Set<Rol>().FindAsync(id);
            if (rol == null) return false;

            rol.Nombre = dto.Nombre;
            rol.Descripcion = dto.Descripcion;
            rol.ActualizadoEn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rol = await _context.Set<Rol>().FindAsync(id);
            if (rol == null) return false;

            _context.Remove(rol);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
