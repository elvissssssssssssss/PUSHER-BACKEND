using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//Models/Productos.cs
namespace apitextil.Models
{
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [StringLength(255)]
        [Column("marca")]
        public string Marca { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("color")]
        public string Color { get; set; } = string.Empty;

        [Required]
        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column("precio_descuento", TypeName = "decimal(10,2)")]
        public decimal? PrecioDescuento { get; set; }

        [Column("stock")]
        public int Stock { get; set; } = 0;

        [Required]
        [StringLength(10)]
        [Column("talla")]
        public string Talla { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        [Column("imagen")]
        public string imagen { get; set; } = string.Empty;

        [StringLength(255)]
        [Column("imagen2")]
        public string? imagen2 { get; set; }

        [StringLength(255)]
        [Column("imagen3")]
        public string? imagen3 { get; set; }

        [StringLength(255)]
        [Column("categoria")]
        public string? Categoria { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

       
    }
}