namespace FERCO.Model
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public DateTime FechaVenta { get; set; }
        public int TotalVenta { get; set; }
        public string? ProductoNombre { get; set; }
    }
}