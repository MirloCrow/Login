using Microsoft.Data.SqlClient;
using System;

namespace FERCO.Data
{
    public static class ConexionBD
    {
        private static readonly string cadena = ConexionLocal.ObtenerCadenaConexion();

        public static SqlConnection ObtenerConexion()
        {
            if (string.IsNullOrWhiteSpace(cadena))
                throw new InvalidOperationException("La cadena de conexión está vacía o no fue proporcionada.");

            try
            {
                return new SqlConnection(cadena);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ConexionBD.ObtenerConexion] {ex.Message}");
                throw;
            }
        }
    }
}
