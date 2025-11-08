
//DTOs/Orders/CreateVentaDto.cs

namespace apitextil.DTOs.Orders
{
    public class CreateVentaDto
    {
        public int UserId { get; set; }
        public int MetodoPagoId { get; set; }
        public decimal Total { get; set; }
    }

}