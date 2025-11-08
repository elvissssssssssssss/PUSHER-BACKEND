using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace apitextil.Models
{
    public class DocumentoEnvio
    {
        [Key]
        public int id { get; set; }

        [Required]
        [ForeignKey("Venta")]
        public int venta_id { get; set; }

        [Required]
        [StringLength(50)]
        public string tipo_documento { get; set; }

        [Required]
        [StringLength(255)]
        public string nombre_archivo { get; set; }

        [Required]
        [StringLength(500)]
        public string ruta_archivo { get; set; }

        public DateTime fecha_subida { get; set; }

        public DateTime creado_en { get; set; }

        public DateTime actualizado_en { get; set; }

        // Propiedad de navegación
        public virtual Venta Venta { get; set; }
    }

}
