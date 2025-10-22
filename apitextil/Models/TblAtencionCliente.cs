// ============================================
// 1. Models/TblAtencionCliente.cs
// ============================================
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using apitextil.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextil.Models
{
    [Table("tblatencion_cliente")]
    public class TblAtencionCliente
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }

        [Required]
        [Column("nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("telefono")]
        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [Column("correo")]
        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [Column("estado")]
        [StringLength(20)]
        public string Estado { get; set; } = "pendiente";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Relación con usuarios (opcional)
        [ForeignKey("UserId")]
        public virtual Tblusuario? Usuario { get; set; }
    }
}