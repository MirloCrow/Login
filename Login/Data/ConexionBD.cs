using Microsoft.Data.SqlClient;

namespace Login.Data
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