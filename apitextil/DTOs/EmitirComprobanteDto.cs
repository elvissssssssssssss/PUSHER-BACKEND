namespace apitextil.DTOs
{
    public class EmitirComprobanteDto
    {
        public int VentaId { get; set; }
        public int TipoComprobante { get; set; } // 1=Factura, 2=Boleta
        public int? NumeroForzado { get; set; }

        // Datos para boleta (personas naturales)
        public string? ClienteDNI { get; set; }
        public string? ClienteNombres { get; set; }
        public string? ClienteApellidos { get; set; }

        // Datos para factura (empresas)
        public string? RUC { get; set; }
        public string? RazonSocial { get; set; }

        // Items del comprobante
        public List<ItemComprobanteDto>? Items { get; set; }
    }
}
