
// Models/DetalleVenta.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;





namespace apitextil.Models
{
    [Table("tbldetalleventa")]
    public class DetalleVenta
    {

        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Relaciones
     
        public Venta Venta { get; set; }
        public Producto Producto { get; set; }
  

        [NotMapped] // Alias para tu lógica interna
        public decimal PrecioUnitario => Precio;
    }

}