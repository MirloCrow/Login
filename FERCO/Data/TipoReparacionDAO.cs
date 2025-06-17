using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class TipoReparacionDAO
    {
        public static List<TipoReparacion> ObtenerTodas()
        {
            List<TipoReparacion> lista = [];

            using (var conn = DAOHelper.AbrirConexionSegura())
            {
                SqlCommand cmd = new("SELECT id_tipo_reparacion, nombre, descripcion FROM TipoReparacion", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new TipoReparacion
                    {
                        IdTipoReparacion = (int)reader["id_tipo_reparacion"],
                        Nombre = reader["nombre"].ToString() ?? "",
                        Descripcion = reader["descripcion"].ToString() ?? ""
                    });
                }
            }

            return lista;
        }
        public static int Insertar(TipoReparacion tipo)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new("INSERT INTO TipoReparacion (nombre, descripcion) OUTPUT INSERTED.id_tipo_reparacion VALUES (@n, @d)", conn);
            cmd.Parameters.AddWithValue("@n", tipo.Nombre);
            cmd.Parameters.AddWithValue("@d", tipo.Descripcion ?? "");
            return (int)cmd.ExecuteScalar();
        }

        public static void AgregarProductoATipo(int idTipo, int idProducto, int cantidad)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new("INSERT INTO TipoReparacionProducto (id_tipo_reparacion, id_producto, cantidad_requerida) VALUES (@idTipo, @idProd, @cant)", conn);
            cmd.Parameters.AddWithValue("@idTipo", idTipo);
            cmd.Parameters.AddWithValue("@idProd", idProducto);
            cmd.Parameters.AddWithValue("@cant", cantidad);
            cmd.ExecuteNonQuery();
        }
    }
}
