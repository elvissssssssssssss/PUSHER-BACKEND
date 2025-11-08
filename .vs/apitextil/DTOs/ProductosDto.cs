using System.ComponentModel.DataAnnotations;

namespace apitextil.DTOs
{// DTOs/ProductosDto.cs
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal? PrecioDescuento { get; set; }
        public int Stock { get; set; }
        public string Talla { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string imagen { get; set; } = string.Empty;
        public string? imagen2 { get; set; }
        public string? imagen3 { get; set; }
        public string? Categoria { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateProductoDto
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "La marca es requerida")]
        [StringLength(255, ErrorMessage = "La marca no puede exceder 255 caracteres")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El color es requerido")]
        [StringLength(255, ErrorMessage = "El color no puede exceder 255 caracteres")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio de descuento debe ser mayor o igual a 0")]
        public decimal? PrecioDescuento { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int Stock { get; set; } = 0;

        [Required(ErrorMessage = "La talla es requerida")]
        [StringLength(10, ErrorMessage = "La talla no puede exceder 10 caracteres")]
        public string Talla { get; set; } = string.Empty;

        [Required(ErrorMessage = "El código es requerido")]
        [StringLength(255, ErrorMessage = "El código no puede exceder 255 caracteres")]
        public string Code { get; set; } = string.Empty;

        // CORREGIDO: Campos para recibir archivos
        public IFormFile? imagen { get; set; }
        public IFormFile? imagen2 { get; set; }
        public IFormFile? imagen3 { get; set; }

        [StringLength(255, ErrorMessage = "La categoría no puede exceder 255 caracteres")]
        public string? Categoria { get; set; }
    }

    public class UpdateProductoDto
    {
        internal string imagenPath;
        internal string imagen2Path;
        internal string imagen3Path;

        [StringLength(255, ErrorMessage = "El nombre no puede exceder 255 caracteres")]
        public string? Nombre { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string? Description { get; set; }

        [StringLength(255, ErrorMessage = "La marca no puede exceder 255 caracteres")]
        public string? Marca { get; set; }

        [StringLength(255, ErrorMessage = "El color no puede exceder 255 caracteres")]
        public string? Color { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal? Precio { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El precio de descuento debe ser mayor o igual a 0")]
        public decimal? PrecioDescuento { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor o igual a 0")]
        public int? Stock { get; set; }

        [StringLength(10, ErrorMessage = "La talla no puede exceder 10 caracteres")]
        public string? Talla { get; set; }

        [StringLength(255, ErrorMessage = "El código no puede exceder 255 caracteres")]
        public string? Code { get; set; }

        public IFormFile? imagen { get; set; }
        public IFormFile? imagen2 { get; set; }
        public IFormFile? imagen3 { get; set; }

        [StringLength(255, ErrorMessage = "La categoría no puede exceder 255 caracteres")]
        public string? Categoria { get; set; }
    }
}
 