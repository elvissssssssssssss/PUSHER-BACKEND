    namespace apitextil.Services
{
    // Services/IAuthService.cs
 
    using global::apitextil.DTOs.apitextil.DTOs;

    namespace apitextil.Services
    {
        public interface IAuthService
        {
            Task<AuthResponseDto> Login(LoginDto loginDto);
            Task<AuthResponseDto> Register(RegisterDto registerDto);
            Task<UserDto?> GetUserById(int id);
            Task<UserDto?> GetUserByEmail(string email);
            Task<bool> UpdateUser(int id, UpdateUserDto updateDto);
            Task<bool> ChangePassword(int id, ChangePasswordDto changePasswordDto);
            Task<bool> DeleteUser(int id);
            Task<List<UserDto>> GetAllUsers();
            string HashPassword(string password);
            bool VerifyPassword(string password, string hashedPassword);
            string GenerateJwtToken(UserDto user);
        }
    }
}