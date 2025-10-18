namespace apitextil.Controllers
{
   
    using global::apitextil.DTOs.apitextil.DTOs;
    using global::apitextil.Services.apitextil.Services;
    // Controllers/AuthController.cs
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;

    namespace apitextil.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class AuthController : ControllerBase
        {
            private readonly IAuthService _authService;

            public AuthController(IAuthService authService)
            {
                _authService = authService;
            }

            [HttpPost("login")]
            public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto loginDto)
            {
                var result = await _authService.Login(loginDto);

                if (string.IsNullOrEmpty(result.token))
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }

            [HttpPost("register")]
            public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
            {
                var result = await _authService.Register(registerDto);

                if (string.IsNullOrEmpty(result.token))
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }

            [HttpGet("profile")]

            public async Task<ActionResult<UserDto>> GetProfile()
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var user = await _authService.GetUserById(userId);

                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(user);
            }

            [HttpPut("profile")]
    
            public async Task<ActionResult> UpdateProfile([FromBody] UpdateUserDto updateDto)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _authService.UpdateUser(userId, updateDto);

                if (!success)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(new { message = "Perfil actualizado exitosamente" });
            }

            [HttpPost("change-password")]
         
            public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _authService.ChangePassword(userId, changePasswordDto);

                if (!success)
                {
                    return BadRequest(new { message = "Contraseña actual incorrecta" });
                }

                return Ok(new { message = "Contraseña cambiada exitosamente" });
            }

            [HttpDelete("profile")]
         
            public async Task<ActionResult> DeleteAccount()
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var success = await _authService.DeleteUser(userId);

                if (!success)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(new { message = "Cuenta eliminada exitosamente" });
            }

            [HttpGet("users")]
           
            public async Task<ActionResult<List<UserDto>>> GetAllUsers()
            {
                var users = await _authService.GetAllUsers();
                return Ok(users);
            }

            [HttpGet("user/{id}")]
        
            public async Task<ActionResult<UserDto>> GetUserById(int id)
            {
                var user = await _authService.GetUserById(id);

                if (user == null)
                {
                    return NotFound(new { message = "Usuario no encontrado" });
                }

                return Ok(user);
            }
        }
    }

}
