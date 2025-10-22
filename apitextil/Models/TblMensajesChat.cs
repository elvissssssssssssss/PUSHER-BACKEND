using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextil.Models
{
    [Table("tblmensajes_chat")]
    public class TblMensajesChat
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("atencion_id")]
        public int AtencionId { get; set; }

        [Required]
        [Column("emisor_tipo")]
        [StringLength(20)]
        public string EmisorTipo { get; set; } = string.Empty; // cliente, admin, sistema

        [Required]
        [Column("emisor_nombre")]
        [StringLength(100)]
        public string EmisorNombre { get; set; } = string.Empty;

        [Required]
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Column("leido")]
        public bool Leido { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relación con atención al cliente
        [ForeignKey("AtencionId")]
        public virtual TblAtencionCliente? AtencionCliente { get; set; }
    }
}
