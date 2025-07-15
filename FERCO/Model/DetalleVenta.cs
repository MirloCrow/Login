namespace FERCO.Model
{
    public class DetalleVenta
    {
        public int IdDetalleVenta { get; set; }
        public int IdVenta { get; set; }
        public int IdProducto { get; set; }
        public int PrecioUnitario { get; set; }
        public int CantidadDetalle { get; set; }
        public int SubtotalDetalle { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
    }
}