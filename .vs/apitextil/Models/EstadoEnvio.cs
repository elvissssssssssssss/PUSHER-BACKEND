

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextil.Models
{
    public class EstadoEnvio
    {
        [Key]
        public int id { get; set; }

        [Required]
        [StringLength(100)]
        public string nombre { get; set; }

        public string descripcion { get; set; }

        public DateTime creado_en { get; set; }

        public DateTime actualizado_en { get; set; }
    }
}

