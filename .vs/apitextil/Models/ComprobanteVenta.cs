using apitextil.Models;
using System;

using System.ComponentModel.DataAnnotations.Schema;

namespace apitextilECommerceAPI.Models
{
    [Table("tblcomprobanteventa")]
    public class ComprobanteVenta
    {
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }

        [Column("tipo_comprobante")]
        public byte TipoComprobante { get; set; } // 1 = Factura, 2 = Boleta

        [Column("serie")]
        public string Serie { get; set; } = string.Empty;

        [Column("numero")]
        public int? Numero { get; set; }

        [Column("enlace_pdf")]
        public string? EnlacePdf { get; set; }

        [Column("codigo_hash")]
        public string? CodigoHash { get; set; }

        [Column("codigo_qr")]
        public string? CodigoQr { get; set; }

        [Column("fecha_generacion")]
        public DateTime FechaGeneracion { get; set; }

        // 🔗 Relación con Venta
        public virtual Venta Venta { get; set; } = null!;
    }
}
