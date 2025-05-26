using Microsoft.Data.SqlClient;

namespace FERCO.Data
{
    public static class ConexionBD
    {
        private static string cadena = ConexionLocal.ObtenerCadenaConexion();

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}