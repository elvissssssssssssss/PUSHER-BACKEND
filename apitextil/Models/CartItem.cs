
// 📁 Models/CartItem.cs
using apitextil.DTOs.apitextil.DTOs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextil.Models
{
    [Table("cart")]
    public class CartItem
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required]
        [StringLength(10)]
        [Column("talla")]
        public string Talla { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Column("quantity")]
        public int Quantity { get; set; } = 1;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public virtual UserDto? User { get; set; }
        public virtual Producto Product { get; set; }
    }
}