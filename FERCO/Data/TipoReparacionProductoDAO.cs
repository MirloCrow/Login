using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class TipoReparacionProductoDAO
    {
        public static List<TipoReparacionProducto> ObtenerProductosPorTipo(int idTipoReparacion)
        {
            List<TipoReparacionProducto> lista = [];

            using (var conn = DAOHelper.AbrirConexionSegura())
            {
                SqlCommand cmd = new(
                    "SELECT trp.id_producto, p.nombre_producto, trp.cantidad_requerida " +
                    "FROM TipoReparacionProducto trp " +
                    "JOIN Producto p ON trp.id_producto = p.id_producto " +
                    "WHERE trp.id_tipo_reparacion = @id", conn);
                cmd.Parameters.AddWithValue("@id", idTipoReparacion);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new TipoReparacionProducto
                    {
                        IdTipoReparacion = idTipoReparacion,
                        IdProducto = (int)reader["id_producto"],
                        NombreProducto = reader["nombre_producto"].ToString() ?? "",
                        CantidadRequerida = (int)reader["cantidad_requerida"]
                    });
                }
            }

            return lista;
        }
    }
}
