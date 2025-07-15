using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FERCO.Model
{
    public class Reparacion
    {
        public int IdReparacion { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = string.Empty;

        public DateTime FechaReparacion { get; set; }
        public int CostoReparacion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
