// Models/MetodoPago.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace apitextilECommerceAPI.Models
{
    [Table("tblmetodopago")]

    public class MetodoPago
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

       
    }
}
