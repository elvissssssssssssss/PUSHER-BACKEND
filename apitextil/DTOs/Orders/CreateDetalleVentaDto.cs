

//DTOs/Orders/CreateDetalleVentaDto.cs

namespace apitextil.DTOs.Orders
{
    public class CreateDetalleVentaDto
    {
        public int ProductoId { get; set; }
        public string? NombreProducto { get; set; } // ✅ Nuevo campo
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
