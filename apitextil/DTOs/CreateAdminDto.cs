namespace apitextil.DTOs
{
    public class CreateAdminDto
    {
        // Nombre completo del administrador
        public string Nombre { get; set; } = string.Empty;

        // Email (correo para iniciar sesión)
        public string Email { get; set; } = string.Empty;

        // Contraseña en texto plano (solo para pruebas locales)
        // En producción, la cifrarás con BCrypt antes de guardar
        public string Password { get; set; } = string.Empty;

        // ID del rol asignado (relacionado con tblroles)
        public int RolId { get; set; }
    }
}
