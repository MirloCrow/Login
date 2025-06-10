namespace FERCO.Model
{
    public class InventarioProducto
    {
        public int IdInventario { get; set; }
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }

        // Esta propiedad es solo para mostrar en la interfaz
        public string Descripcion { get; set; } = "";
    }
}
