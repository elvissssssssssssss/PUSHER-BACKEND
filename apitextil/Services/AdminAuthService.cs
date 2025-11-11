using apitextil.Models;
using apitextil.Data;
using apitextil.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace apitextil.Services
{
    public class AdminAuthService
    {
        private readonly EcommerceContext _context;
        private readonly IConfiguration _config;

        public AdminAuthService(EcommerceContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<LoginAdmin?> AuthenticateAsync(string email, string password)
        {
            var admin = await _context.LoginAdmins.FirstOrDefaultAsync(a => a.Email == email && a.Activo);
            if (admin == null) return null;

            // === 🔐 Verificación de contraseña ===
            // En entorno local: simple comparación (texto plano o SHA256)
            // En producción: se recomienda usar BCrypt (ver línea comentada más abajo)

            if (VerifyPasswordLocal(password, admin.PasswordHash))
            // if (VerifyPasswordBcrypt(password, admin.PasswordHash))  // ← habilita esto en producción
            {
                admin.UltimoAcceso = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return admin;
            }

            return null;
        }

        // ========================================
        // 💻 MODO LOCAL – COMPARACIÓN SIMPLE
        // ========================================
        private bool VerifyPasswordLocal(string password, string storedHash)
        {
            // 🔹 Opción A: texto plano (más rápido para pruebas)
            return password == storedHash;

            // 🔹 Opción B: SHA256 (más realista, sin usar bcrypt)
            /*
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hashString == storedHash;
            */
        }

        // ========================================
        // 🔐 MODO PRODUCCIÓN – USANDO BCrypt (seguro)
        // ========================================
        /*
        private bool VerifyPasswordBcrypt(string password, string storedHash)
        {
            // Requiere instalar:
            // dotnet add package BCrypt.Net-Next
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
        */
    }
}