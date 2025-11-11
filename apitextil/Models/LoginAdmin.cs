using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextil.Models
{
    [Table("tblloginadmin")]
    public class LoginAdmin
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        [Required, MaxLength(100)]
        public string Nombre { get; set; }

        [Column("email")]
        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Column("password_hash")]
        [Required, MaxLength(255)]
        public string PasswordHash { get; set; }

        [Column("rol")]
        [Required, MaxLength(50)]
        public string Rol { get; set; }  // SuperAdmin, Administrador, GestorComercial, Soporte, Contador
        [Column("rol_id")]
        public int? RolId { get; set; }

        [Column("ultimo_acceso")]
        public DateTime? UltimoAcceso { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [Column("actualizado_en")]
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;

        // Relaciones
        [ForeignKey("RolId")]
        public Rol? RolNavigation { get; set; }

    }
}
