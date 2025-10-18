
using System;



namespace apitextil.DTOs
{
    public class SeguimientoEnvioDto
    {

        public int id { get; set; }
        public int venta_id { get; set; }
        public int estado_id { get; set; }
        public string ubicacion_actual { get; set; }
        public string observaciones { get; set; }
        public DateTime fecha_cambio { get; set; }
        public bool confirmado_por_cliente { get; set; }
        public DateTime? fecha_confirmacion { get; set; }
        public string estado_nombre { get; set; }
        public string estado_descripcion { get; set; }
    }
}
public class CreateSeguimientoEnvioDto
{
    public int venta_id { get; set; }
    public int estado_id { get; set; }
    public string ubicacion_actual { get; set; }
    public string observaciones { get; set; }
}

public class ConfirmarEntregaDto
{
    public int venta_id { get; set; }
    public int usuario_id { get; set; }
}
