using apitextil.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextilECommerceAPI.Models
{
    [Table("tblvouchers")]
    public class TblVoucher
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        // ✅ CAMBIAR de long a int para que coincida con Venta.Id
        [Column("order_id")]
        public int OrderId { get; set; }  // ⚠️ Antes era long, ahora es int

        [Column("voucher_archivo")]
        [Required]
        [StringLength(500)]
        public string VoucherArchivo { get; set; }

        [Column("numero_operacion")]
        [StringLength(100)]
        public string? NumeroOperacion { get; set; }

        [Column("estado")]
        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "pendiente";

        [Column("observacion")]
        public string? Observacion { get; set; }

        [Column("fecha_subida")]
        public DateTime FechaSubida { get; set; }

        [Column("fecha_revision")]
        public DateTime? FechaRevision { get; set; }

        // Relación con Venta
        [ForeignKey("OrderId")]
        public virtual Venta Venta { get; set; }
    }
}
