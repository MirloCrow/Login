namespace FERCO.Model
{
    public class InventarioProductoView
    {
        public int IdInventario { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public string DescripcionUbicacion { get; set; }

        public override string ToString()
        {
            return $"{DescripcionUbicacion} (Stock: {Cantidad})";
        }
    }
}
