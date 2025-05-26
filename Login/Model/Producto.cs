namespace Login.Model
{
    public class Producto
    {
        public int IdProducto { get; set; }

        public string NombreProducto { get; set; } = "";
        public string DescripcionProducto { get; set; } = "";

        public int PrecioProducto { get; set; }
        public int StockProducto { get; set; }

        public int IdProveedor { get; set; }
        public int IdCategoria { get; set; }

        public override string ToString() =>
            $"{NombreProducto} | ${PrecioProducto} | Stock: {StockProducto}";
    }
}
