
using apitextil.Data;
using apitextil.DTOs.Orders;
using apitextil.Models;
using apitextilECommerceAPI.Models;
using MercadoPago.Client.Preference;
using MercadoPago.Config;
using MercadoPago.Resource.Preference;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace apitextil.Services
{
    public class VentaService : IVentaService
    {
        private readonly EcommerceContext _context;
        private readonly IConfiguration _configuration;

        public VentaService(EcommerceContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Venta> CrearVentaAsync(CreateVentaDto dto)
        {
            var venta = new Venta
            {
                UserId = dto.UserId,
                MetodoPagoId = dto.MetodoPagoId,
                Total = dto.Total,
                FechaVenta = DateTime.Now
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            return venta;
        }
        public async Task<ComprobanteVenta?> ObtenerComprobantePorVentaAsync(int ventaId)
        {
            return await _context.ComprobantesVenta
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.VentaId == ventaId);
        }
        public async Task<VentaDto> CrearVentaConDetallesAsync(CreateVentaConDetallesDto dto)
        {
            // Guardar venta en base de datos
            var venta = new Venta
            {
                UserId = dto.UserId,
                MetodoPagoId = dto.MetodoPagoId,
                Total = dto.Total,
                FechaVenta = DateTime.Now
            };

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            // Guardar detalles de venta
            foreach (var d in dto.Detalles)
            {
                var detalle = new DetalleVenta
                {
                    VentaId = venta.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario
                };
                _context.DetalleVentas.Add(detalle);
            }
            await _context.SaveChangesAsync();

            // Configurar MercadoPago
            MercadoPagoConfig.AccessToken = _configuration["MercadoPago:AccessToken"];

            var request = new PreferenceRequest
            {
                Items = dto.Detalles.Select(d => new PreferenceItemRequest
                {
                    Title = d.NombreProducto ?? $"Producto {d.ProductoId}",
                    Quantity = d.Cantidad,
                    UnitPrice = d.PrecioUnitario
                }).ToList(),
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = "https://tusitio.com/success",
                    Failure = "https://tusitio.com/failure",
                    Pending = "https://tusitio.com/pending"
                },
                AutoReturn = "approved"
            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            // Retornar DTO con InitPoint
            return new VentaDto
            {
                Id = venta.Id,
                UserId = venta.UserId,
                MetodoPagoId = venta.MetodoPagoId,
                Total = venta.Total,
                FechaVenta = venta.FechaVenta,
                InitPoint = preference.InitPoint, // ✅ URL de pago
                Detalles = dto.Detalles.Select(d => new DetalleVentaDto
                {
                    ProductoId = d.ProductoId,
                    NombreProducto = d.NombreProducto,
                    Cantidad = d.Cantidad,
                    Precio = d.PrecioUnitario
                }).ToList()
            };
        }

        public async Task<IEnumerable<VentaDto>> ObtenerVentasAsync()
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
                .Select(v => new VentaDto
                {
                    Id = v.Id,
                    UserId = v.UserId,
                    MetodoPagoId = v.MetodoPagoId,
                    Total = v.Total,
                    FechaVenta = v.FechaVenta,
                    Detalles = v.Detalles.Select(d => new DetalleVentaDto
                    {
                        ProductoId = d.ProductoId,
                        NombreProducto = d.Producto.Nombre,
                        Cantidad = d.Cantidad,
                        Precio = d.Precio
                    }).ToList()
                }).ToListAsync();
        }





        public async Task<Venta?> ObtenerVentaCompletaAsync(int ventaId)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(v => v.Comprobante)
                .FirstOrDefaultAsync(v => v.Id == ventaId);
        }


        public async Task<int> ObtenerSiguienteNumeroComprobanteAsync(int tipoComprobante)
        {
            var ultimo = await _context.ComprobantesVenta
                .Where(c => c.TipoComprobante == tipoComprobante)
                .OrderByDescending(c => c.Numero)
                .FirstOrDefaultAsync();

            return ultimo?.Numero + 1 ?? 1; // si no existe, arranca en 1
        }

        public async Task ActualizarComprobanteVentaAsync(int ventaId, ComprobanteVenta comp)
        {
            var venta = await _context.Ventas
                .Include(v => v.Comprobante)
                .FirstOrDefaultAsync(v => v.Id == ventaId);

            if (venta == null) throw new Exception($"No existe la venta {ventaId}");

            // Si ya existe comprobante, actualízalo; si no, créalo
            if (venta.Comprobante != null)
            {
                venta.Comprobante.Serie = comp.Serie;
                venta.Comprobante.Numero = comp.Numero;
                venta.Comprobante.EnlacePdf = comp.EnlacePdf;
                venta.Comprobante.CodigoHash = comp.CodigoHash;
                venta.Comprobante.CodigoQr = comp.CodigoQr;
                venta.Comprobante.TipoComprobante = comp.TipoComprobante;
                venta.Comprobante.FechaGeneracion = DateTime.Now;
            }
            else
            {
                comp.VentaId = venta.Id;
                _context.ComprobantesVenta.Add(comp);
            }

            await _context.SaveChangesAsync();
        }




    }
}