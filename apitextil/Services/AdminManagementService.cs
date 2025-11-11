using apitextil.Data;
using apitextil.Models;
using Microsoft.EntityFrameworkCore;

namespace apitextil.Services
{
    public class AdminManagementService
    {
        private readonly EcommerceContext _context;

        public AdminManagementService(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<List<LoginAdmin>> GetAllAsync()
            => await _context.LoginAdmins.AsNoTracking().OrderBy(a => a.Nombre).ToListAsync();

        public async Task<LoginAdmin?> GetByIdAsync(int id)
            => await _context.LoginAdmins.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        public async Task<LoginAdmin> CreateAsync(string nombre, string email, string password, int rolId)
        {
            var rol = await _context.Roles.FindAsync(rolId);
            if (rol == null) throw new Exception("Rol no encontrado.");

            var admin = new LoginAdmin
            {
                Nombre = nombre,
                Email = email,
                PasswordHash = password,
                Rol = rol.Nombre,
                RolId = rol.Id,
                Activo = true,
                CreadoEn = DateTime.UtcNow
            };

            _context.LoginAdmins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<LoginAdmin?> ToggleActiveAsync(int id)
        {
            var admin = await _context.LoginAdmins.FirstOrDefaultAsync(a => a.Id == id);
            if (admin == null) return null;

            admin.Activo = !admin.Activo;
            admin.ActualizadoEn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var admin = await _context.LoginAdmins.FindAsync(id);
            if (admin == null) return false;

            _context.LoginAdmins.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
