namespace FERCO.Model
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public string? ClienteNombre { get; set; } // Para mostrar en boleta
        public DateTime FechaVenta { get; set; }
        public int TotalVenta { get; set; }
        public string? ProductoNombre { get; set; }
    }
}