using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextilECommerceAPI.Models
{
    [Table("tblconfig_pago")]
    public class TblConfigPago
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("cuenta_soles")]
        [StringLength(50)]
        public string CuentaSoles { get; set; }

        [Column("cci")]
        [StringLength(50)]
        public string Cci { get; set; }

        [Column("cuenta_activa")]
        public bool CuentaActiva { get; set; }

        [Column("yape_qr")]
        [StringLength(255)]
        public string YapeQr { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
