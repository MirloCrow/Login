using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FERCO.Model
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdProveedor { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public int TotalPedido { get; set; }

        public string NombreProveedor { get; set; } = ""; // Para mostrar
        public List<DetallePedido> Detalles { get; set; } = [];
    }
}
