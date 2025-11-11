
    namespace apitextil.DTOs
    {
        public class AuthAdminDto
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class AdminResponseDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Email { get; set; }
            public string Rol { get; set; }
            public string Token { get; set; }
        }
    }
