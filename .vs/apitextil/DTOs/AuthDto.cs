namespace apitextil.DTOs
{
    // DTOs/AuthDto.cs
    using System.ComponentModel.DataAnnotations;

    namespace apitextil.DTOs
    {
        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public string email { get; set; } = string.Empty;

            [Required]
            public string password { get; set; } = string.Empty;
        }

        public class RegisterDto
        {
            [Required]
            [EmailAddress]
            public string email { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string password { get; set; } = string.Empty;

            [Required]
            [StringLength(50)]
            public string nombre { get; set; } = string.Empty;

            [Required]
            [StringLength(50)]
            public string apellido { get; set; } = string.Empty;

            [Required]
            public DateTime fecha_nacimiento { get; set; }
        }

        public class UserDto
        {
            public int id { get; set; }
            public string email { get; set; } = string.Empty;
            public string nombre { get; set; } = string.Empty;
            public string apellido { get; set; } = string.Empty;
            public DateTime fecha_nacimiento { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
        }

        public class AuthResponseDto
        {
            public string token { get; set; } = string.Empty;
            public UserDto user { get; set; } = new UserDto();
            public string message { get; set; } = string.Empty;
        }

        public class UpdateUserDto
        {
            [StringLength(50)]
            public string? nombre { get; set; }

            [StringLength(50)]
            public string? apellido { get; set; }

            public DateTime? fecha_nacimiento { get; set; }
        }

        public class ChangePasswordDto
        {
            [Required]
            public string currentPassword { get; set; } = string.Empty;

            [Required]
            [MinLength(6)]
            public string newPassword { get; set; } = string.Empty;
        }
    }

}
