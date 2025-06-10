namespace FERCO.Model
{
    public class Producto
    {
        public int IdProducto { get; set; }

        public string NombreProducto { get; set; } = "";
        public string DescripcionProducto { get; set; } = "";

        public int PrecioProducto { get; set; }

        public int IdProveedor { get; set; }
        public int IdCategoria { get; set; }

        public int StockTotal => UbicacionesConStock?.Sum(u => u.Cantidad) ?? 0;

        public List<InventarioProducto>? UbicacionesConStock { get; set; }

        public override string ToString() =>
            $"{NombreProducto} | ${PrecioProducto} | Stock total: {StockTotal}";
    }
}
