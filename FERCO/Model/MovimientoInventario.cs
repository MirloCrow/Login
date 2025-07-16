namespace FERCO.Model
{
    public class MovimientoInventario
    {
        public int IdMovimiento { get; set; }
        public int IdProducto { get; set; }
        public int IdInventario { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public string TipoMovimiento { get; set; } = "";
        public int Cantidad { get; set; }
        public string? Motivo { get; set; }
        public int? IdReferencia { get; set; }
        // Solo para mostrar en la UI
        public string? DescripcionInventario { get; set; }
    }
}
