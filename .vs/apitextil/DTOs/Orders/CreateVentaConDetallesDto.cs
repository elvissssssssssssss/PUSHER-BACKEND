using System.Collections.Generic;



// DTOs/Orders/CreateVentaConDetallesDto.cs 
namespace apitextil.DTOs.Orders

{
    public class CreateVentaConDetallesDto
    {
        public int UserId { get; set; }
        public int MetodoPagoId { get; set; }
        public decimal Total { get; set; }
        public List<CreateDetalleVentaDto>? Detalles { get; set; }
    }
}
