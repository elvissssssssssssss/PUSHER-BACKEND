
// DTOs/Orders/VentaDto.css



namespace apitextil.DTOs.Orders
{
    public class VentaDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UsuarioEmail { get; set; } = string.Empty;
        public int MetodoPagoId { get; set; }
        public string MetodoPagoNombre { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime FechaVenta { get; set; }
        // ✅ NUEVO: URL de pago de Mercado Pago
        public string? InitPoint { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new();
    }

    public class DetalleVentaDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string NombreProducto { get; set; }  // <--- nuevo
        public decimal Precio { get; set; }
    }
}
