// Services/IVentaService.cs
using apitextil.DTOs.Orders;
using apitextil.Models;
using apitextilECommerceAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks;





namespace apitextil.Services
{
    public interface IVentaService
{
    Task<Venta> CrearVentaAsync(CreateVentaDto dto);
        Task<VentaDto> CrearVentaConDetallesAsync(CreateVentaConDetallesDto dto);
        Task<IEnumerable<VentaDto>> ObtenerVentasAsync();

        // ✅ Nuevo método para crear venta con pago MercadoPago

        // 👇 Nuevos métodos
        Task<Venta?> ObtenerVentaCompletaAsync(int ventaId);
        Task<int> ObtenerSiguienteNumeroComprobanteAsync(int tipoComprobante);
        Task ActualizarComprobanteVentaAsync(int ventaId, ComprobanteVenta comp);
        Task<ComprobanteVenta?> ObtenerComprobantePorVentaAsync(int ventaId);

    }
}
