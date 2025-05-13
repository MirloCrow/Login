using Microsoft.Data.SqlClient;

namespace Login.Data
{
    public class ConexionBD
    {
        private static string cadena = ConexionLocal.ObtenerCadenaConexion();

        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(cadena);
            conexion.Open();
            return conexion;
        }
    }
}
