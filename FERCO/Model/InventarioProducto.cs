namespace FERCO.Model
{
    public class InventarioProducto
    {
        public int IdInventario { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }

        // Esta propiedad es útil para mostrar el nombre del inventario en la UI
        public string DescripcionUbicacion { get; set; } = "";

        public override string ToString()
        {
            return $"{DescripcionUbicacion} (Stock: {Cantidad})";
        }
    }
}
