using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
//
namespace apitextil.DTOs
{
    // DTO principal para recibir el voucher completo
    public class VoucherRequestDto
    {
        [Required(ErrorMessage = "El voucher es obligatorio")]
        public IFormFile Voucher { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "UserId debe ser mayor a 0")]
        public int UserId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        [StringLength(100)]
        public string? NumeroOperacion { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50)]
        public string ClienteNombres { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(50)]
        public string ClienteApellidos { get; set; }

        [StringLength(20)]  // ✅ Cambiado de 8 a 20
        public string? ClienteDNI { get; set; }

        [Required]
        [StringLength(20)]
        public string? TipoComprobante { get; set; }

        [StringLength(11)]
        public string? Ruc { get; set; }

        [StringLength(100)]
        public string? RazonSocial { get; set; }

        [Required(ErrorMessage = "Los detalles son obligatorios")]
        public string Detalles { get; set; }
    }

    // DTO para los detalles de venta
    public class DetalleVentaDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    // DTO de respuesta exitosa
    public class VoucherResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int VentaId { get; set; }
        public int OrderId { get; set; }  // ✅ Cambiado de long a int
        public string UsuarioEmail { get; set; }
    }

    // DTO de respuesta de error
    public class ErrorResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }

    // DTO para la configuración de pago
    public class ConfigPagoDto
    {
        public string CuentaSoles { get; set; }
        public string Cci { get; set; }
        public bool CuentaActiva { get; set; }
        public string YapeQr { get; set; }
    }
}
