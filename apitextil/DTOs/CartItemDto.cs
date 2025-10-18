
// 📁 Models/DTOs/CartItemDto.cs
using System.ComponentModel.DataAnnotations;

namespace apitextil.Models.DTOs
{
    public class CartItemDto
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string? Talla { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Color { get; set; } = "";
        public decimal? PrecioDescuento { get; set; }
        // Información del producto (calculada)
        public string? ProductName { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public decimal TotalPrice { get; set; } // Precio total del item (precio * cantidad)
    }

    public class AddToCartDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(10)]
        public string? Talla { get; set; }

        [Required]
        [Range(1, 10)]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        [Required]
        [Range(1, 10)]
        public int Quantity { get; set; }
    }

    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    }
}
