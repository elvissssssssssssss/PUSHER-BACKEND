namespace apitextil.DTOs
{
    public class ChatMessageDto
    {
        public string Usuario { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;  // "cliente" o "asesor"
        public string Fecha { get; set; } = string.Empty;
    }
}
