using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FERCO.Model
{
    /// <summary>
    /// Representa los parámetros de configuración global del sistema.
    /// Estos valores se guardan en un archivo JSON y se cargan al iniciar la aplicación.
    /// </summary>
    public class ConfiguracionApp
    {
        /// <summary>
        /// Define cuál es el umbral mínimo de stock considerado como bajo.
        /// Este valor es utilizado por el sistema para mostrar alertas visuales.
        /// </summary>
        public int UmbralStockBajo { get; set; } = 5;

        // Aquí puedes agregar más configuraciones en el futuro, por ejemplo:
        // public string RutaDeRespaldos { get; set; } = "C:\\RespaldoFERCO";
        // public bool HabilitarNotificaciones { get; set; } = true;
    }
}


