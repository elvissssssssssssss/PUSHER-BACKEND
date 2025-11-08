using apitextil.Data;
using apitextil.Models;
using apitextil.Services;
using apitextil.Services.apitextil.Services;
using global::apitextil.DTOs.apitextil.DTOs;
// Services/AuthService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace apitextil.Services
{
    public class AuthService : IAuthService
    {
        private readonly EcommerceContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(EcommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
            var user = await _context.tblusuarios
                .FirstOrDefaultAsync(u => u.email == loginDto.email);

            if (user == null || !VerifyPassword(loginDto.password, user.password))
            {
                return new AuthResponseDto
                {
                    message = "Credenciales inválidas"
                };
            }

            var userDto = MapToUserDto(user);
            var token = GenerateJwtToken(userDto);

            return new AuthResponseDto
            {
                token = token,
                user = userDto,
                message = "Login exitoso"
            };
        }

        public async Task<AuthResponseDto> Register(RegisterDto registerDto)
        {
            if (await _context.tblusuarios.AnyAsync(u => u.email == registerDto.email))
            {
                return new AuthResponseDto
                {
                    message = "El email ya está registrado"
                };
            }

            var user = new Tblusuario
            {
                email = registerDto.email,
                password = HashPassword(registerDto.password),
                nombre = registerDto.nombre,
                apellido = registerDto.apellido,
                fecha_nacimiento = registerDto.fecha_nacimiento,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            _context.tblusuarios.Add(user);
            await _context.SaveChangesAsync();

            var userDto = MapToUserDto(user);
            var token = GenerateJwtToken(userDto);

            return new AuthResponseDto
            {
                token = token,
                user = userDto,
                message = "Usuario registrado exitosamente"
            };
        }

        public async Task<UserDto?> GetUserById(int id)
        {
            var user = await _context.tblusuarios.FindAsync(id);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<UserDto?> GetUserByEmail(string email)
        {
            var user = await _context.tblusuarios
                .FirstOrDefaultAsync(u => u.email == email);
            return user != null ? MapToUserDto(user) : null;
        }

        public async Task<bool> UpdateUser(int id, UpdateUserDto updateDto)
        {
            var user = await _context.tblusuarios.FindAsync(id);
            if (user == null) return false;

            if (!string.IsNullOrEmpty(updateDto.nombre))
                user.nombre = updateDto.nombre;

            if (!string.IsNullOrEmpty(updateDto.apellido))
                user.apellido = updateDto.apellido;

            if (updateDto.fecha_nacimiento.HasValue)
                user.fecha_nacimiento = updateDto.fecha_nacimiento.Value;

            user.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePassword(int id, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.tblusuarios.FindAsync(id);
            if (user == null) return false;

            if (!VerifyPassword(changePasswordDto.currentPassword, user.password))
                return false;

            user.password = HashPassword(changePasswordDto.newPassword);
            user.updated_at = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.tblusuarios.FindAsync(id);
            if (user == null) return false;

            _context.tblusuarios.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _context.tblusuarios.ToListAsync();
            return users.Select(MapToUserDto).ToList();
        }

        public string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            var hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);

            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(hashedPassword);
                var salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
                var hash = pbkdf2.GetBytes(32);

                for (int i = 0; i < 32; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateJwtToken(UserDto user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? "your-secret-key-here-must-be-at-least-32-characters"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.nombre),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.apellido),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"] ?? "apitextil",
                audience: _configuration["JwtSettings:Audience"] ?? "apitextil-client",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private UserDto MapToUserDto(Tblusuario user)
        {
            return new UserDto
            {
                id = user.id,
                email = user.email,
                nombre = user.nombre,
                apellido = user.apellido,
                fecha_nacimiento = user.fecha_nacimiento,
                created_at = user.created_at,
                updated_at = user.updated_at
            };
        }
    }
}


