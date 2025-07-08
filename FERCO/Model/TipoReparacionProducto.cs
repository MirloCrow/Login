namespace FERCO.Model
{
    public class TipoReparacionProducto
    {
        public int IdTipoReparacion { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = "";
        public int CantidadRequerida { get; set; }
    }
}
