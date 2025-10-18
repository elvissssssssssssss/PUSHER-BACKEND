using System;



namespace apitextil.DTOs
{
    public class DocumentoEnvioDto
    {

        public int id { get; set; }
        public int venta_id { get; set; }
        public string tipo_documento { get; set; }
        public string nombre_archivo { get; set; }
        public string ruta_archivo { get; set; }
        public DateTime fecha_subida { get; set; }
    }

    public class CreateDocumentoEnvioDto
    {
        public int venta_id { get; set; }
        public string tipo_documento { get; set; }
        public string nombre_archivo { get; set; }
        public string ruta_archivo { get; set; }
    }

    public class UploadDocumentoDto
    {
        public int venta_id { get; set; }
        public string tipo_documento { get; set; }
        public IFormFile archivo { get; set; }
    }
}