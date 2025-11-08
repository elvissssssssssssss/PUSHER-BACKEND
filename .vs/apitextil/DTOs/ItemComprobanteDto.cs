namespace apitextil.DTOs
{
    public class ItemComprobanteDto
    {
        public string unidad_de_medida { get; set; } = "NIU";
        public string descripcion { get; set; } = string.Empty;
        public decimal cantidad { get; set; }
        public decimal valor_unitario { get; set; }
        public decimal precio_unitario { get; set; }
        public decimal subtotal { get; set; }
        public int tipo_de_igv { get; set; }
        public decimal igv { get; set; }
        public decimal total { get; set; }
    }

}
