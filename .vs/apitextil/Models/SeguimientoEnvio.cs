using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace apitextil.Models
{
    public class SeguimientoEnvio
    {
        [Key]
        public int id { get; set; }

        [Required]
        [ForeignKey("Venta")]
        public int venta_id { get; set; }

        [Required]
        [ForeignKey("EstadoEnvio")]
        public int estado_id { get; set; }

        [StringLength(255)]
        public string ubicacion_actual { get; set; }

        public string observaciones { get; set; }

        public DateTime fecha_cambio { get; set; }

        public bool confirmado_por_cliente { get; set; }

        public DateTime? fecha_confirmacion { get; set; }

        public DateTime creado_en { get; set; }

        public DateTime actualizado_en { get; set; }

        // Propiedades de navegación
        public virtual Venta Venta { get; set; }
        public virtual EstadoEnvio EstadoEnvio { get; set; }
    }
}
