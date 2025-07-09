using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FERCO.Model
{
    public class DetallePedido
    {
        public int IdDetallePedido { get; set; }
        public int IdPedido { get; set; }
        public int IdProducto { get; set; }
        public string? NombreProducto { get; set; } // Para mostrar en UI
        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }

        public int Subtotal => Cantidad * PrecioUnitario;
    }

}
