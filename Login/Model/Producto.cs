namespace Login.Model
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string NombreProducto { get; set; } = default!;
        public int PrecioProducto { get; set; }
        public int StockProducto { get; set; }

        public override string ToString() => $"{NombreProducto} - Stock: {StockProducto}";
    }
}
