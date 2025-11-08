// Models/TblEnvios.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextil.Models
{
    [Table("tblenvios")]
    public class TblEnvios
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column("direccion")]
        public string Direccion { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("region")]
        public string Region { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("provincia")]
        public string Provincia { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("localidad")]
        public string Localidad { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("dni")]
        public string Dni { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
