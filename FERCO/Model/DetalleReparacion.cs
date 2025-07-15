namespace FERCO.Model
{
    public class DetalleReparacion
    {
        public int IdDetalleReparacion { get; set; }
        public int IdReparacion { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }

        // Este campo es solo para mostrar en UI
        public string NombreProducto { get; set; } = "";
    }
}
