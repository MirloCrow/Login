using System;
using Microsoft.Data.SqlClient;

namespace FERCO.Data
{
    public static class LogDAO
    {
        public static void Registrar(string entidad, string accion, string descripcion)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            var cmd = new SqlCommand(@"
                INSERT INTO Logs (Entidad, Accion, Descripcion, Fecha)
                VALUES (@entidad, @accion, @descripcion, @fecha)", conn);

            cmd.Parameters.AddWithValue("@entidad", entidad);
            cmd.Parameters.AddWithValue("@accion", accion);
            cmd.Parameters.AddWithValue("@descripcion", descripcion);
            cmd.Parameters.AddWithValue("@fecha", DateTime.Now);

            cmd.ExecuteNonQuery();
        }
    }
}
