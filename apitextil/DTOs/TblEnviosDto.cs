
// DTOs/TblEnviosDto.cs
using System.ComponentModel.DataAnnotations;

namespace apitextil.DTOs
{
    public class TblEnviosCreateDto
    {
        [Required(ErrorMessage = "El campo UserId es requerido")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [MaxLength(255, ErrorMessage = "La dirección no puede exceder los 255 caracteres")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "La región es requerida")]
        [MaxLength(100, ErrorMessage = "La región no puede exceder los 100 caracteres")]
        public string Region { get; set; }

        [Required(ErrorMessage = "La provincia es requerida")]
        [MaxLength(100, ErrorMessage = "La provincia no puede exceder los 100 caracteres")]
        public string Provincia { get; set; }

        [Required(ErrorMessage = "La localidad es requerida")]
        [MaxLength(100, ErrorMessage = "La localidad no puede exceder los 100 caracteres")]
        public string Localidad { get; set; }

        [Required(ErrorMessage = "El DNI es requerido")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos")]
        public string Dni { get; set; }

        [Required(ErrorMessage = "El teléfono es requerido")]
        [RegularExpression(@"^\+?51\d{9}$", ErrorMessage = "El teléfono debe tener el formato correcto (+51xxxxxxxxx)")]
        public string Telefono { get; set; }
    }

    public class TblEnviosUpdateDto
    {
        [MaxLength(255, ErrorMessage = "La dirección no puede exceder los 255 caracteres")]
        public string? Direccion { get; set; }

        [MaxLength(100, ErrorMessage = "La región no puede exceder los 100 caracteres")]
        public string? Region { get; set; }

        [MaxLength(100, ErrorMessage = "La provincia no puede exceder los 100 caracteres")]
        public string? Provincia { get; set; }

        [MaxLength(100, ErrorMessage = "La localidad no puede exceder los 100 caracteres")]
        public string? Localidad { get; set; }

        [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener exactamente 8 dígitos")]
        public string? Dni { get; set; }

        [RegularExpression(@"^\+?51\d{9}$", ErrorMessage = "El teléfono debe tener el formato correcto (+51xxxxxxxxx)")]
        public string? Telefono { get; set; }
    }

    public class TblEnviosResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Direccion { get; set; }
        public string Region { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Dni { get; set; }
        public string Telefono { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
