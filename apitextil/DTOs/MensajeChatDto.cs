using System;
using System.ComponentModel.DataAnnotations;
//DTOs/MensajeChatDto.cs
namespace apitextil.DTOs
{
    public class MensajeChatDto
    {
        [Required]
        public int AtencionId { get; set; }

        [Required]
        [StringLength(20)]
        public string EmisorTipo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EmisorNombre { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Mensaje { get; set; } = string.Empty;
    }

    public class MensajeChatResponseDto
    {
        public int Id { get; set; }
        public int AtencionId { get; set; }
        public string EmisorTipo { get; set; } = string.Empty;
        public string EmisorNombre { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Leido { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class HistorialChatDto
    {
        public AtencionClienteResponseDto? MensajeOriginal { get; set; }
        public List<MensajeChatResponseDto> Mensajes { get; set; } = new List<MensajeChatResponseDto>();
    }
}