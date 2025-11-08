
using apitextilECommerceAPI.Models;
using System;
// Models/Venta.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using apitextil.DTOs.Orders;




namespace apitextil.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("tblventa")]
    public class Venta
    {
        

        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("metodo_pago_id")]
        public int MetodoPagoId { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        // La columna en DB tiene DEFAULT CURRENT_TIMESTAMP (NOT NULL)
        [Column("fecha_venta", TypeName = "timestamp")]
        public DateTime FechaVenta { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }


        [ForeignKey("UserId")]
        public Tblusuario User { get; set; }

        [ForeignKey("MetodoPagoId")]
        public MetodoPago MetodoPago { get; set; }


        public List<DetalleVenta> Detalles { get; set; } = new();


        public virtual ComprobanteVenta? Comprobante { get; set; }



        //public object MetodoPagoNombre { get; internal set; }
        // public object User { get; internal set; }
    }

}