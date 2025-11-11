namespace apitextil.DTOs
{
    public class RolDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class CreateRolDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }

    public class UpdateRolDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
    }
}
