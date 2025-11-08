
// Models/tblusuario.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextil.Models
{
    [Table("tblusuarios")]
    public class Tblusuario
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(50)]
        public string email { get; set; } = string.Empty;
       // public string Email { get; internal set; }
        [Required]
        [StringLength(255)]
        public string password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string apellido { get; set; } = string.Empty;


        [Column("fecha_nacimiento")]
        public DateTime fecha_nacimiento { get; set; }

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime updated_at { get; set; } = DateTime.Now;
    }
}
