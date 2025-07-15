namespace FERCO.Model
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public int RutCliente { get; set; } // solo número, sin DV
        public bool EstadoCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public string DireccionCliente { get; set; } = string.Empty;
        public int TelefonoCliente { get; set; }

        // Historial
        public List<Reparacion> Reparaciones { get; set; } = [];
        public List<Venta> Ventas { get; set; } = [];

    }
}