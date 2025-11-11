//controllers/AdminAuthController.cs
using apitextil.DTOs;
using apitextil.Models;
using apitextil.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace apitextil.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminAuthController : ControllerBase
    {
        private readonly AdminAuthService _adminAuthService;
        private readonly IConfiguration _config;

        public AdminAuthController(AdminAuthService adminAuthService, IConfiguration config)
        {
            _adminAuthService = adminAuthService;
            _config = config;
        }

        // Endpoint público para iniciar sesión
        [HttpPost("login")]
        public async Task<ActionResult<AdminResponseDto>> Login([FromBody] AuthAdminDto dto)
        {
            var admin = await _adminAuthService.AuthenticateAsync(dto.Email, dto.Password);

            if (admin == null)
                return Unauthorized(new { message = "Credenciales incorrectas o usuario inactivo" });

            var token = GenerateJwtToken(admin);

            return new AdminResponseDto
            {
                Id = admin.Id,
                Nombre = admin.Nombre,
                Email = admin.Email,
                Rol = admin.Rol,
                Token = token
            };
        }

        // Método privado para generar el token JWT
        private string GenerateJwtToken(LoginAdmin admin)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, admin.Email),
                new Claim("rol", admin.Rol),
                new Claim("id", admin.Id.ToString())
            };

            // Ajustado a tu configuración appsettings.json -> "JwtSettings"
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryHours = double.TryParse(_config["JwtSettings:ExpiryInHours"], out double result)
                ? result : 8;

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(expiryHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
