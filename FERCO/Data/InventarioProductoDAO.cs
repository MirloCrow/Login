using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class InventarioProductoDAO
    {
        public static List<InventarioProducto> ObtenerProductosPorUbicacion(int idInventario)
        {
            List<InventarioProducto> lista = [];

            using (var conn = DAOHelper.AbrirConexionSegura())
            {
                SqlCommand cmd = new("SELECT ip.id_inventario, ip.id_producto, p.nombre_producto, ip.cantidad " +
                                     "FROM InventarioProducto ip " +
                                     "JOIN Producto p ON ip.id_producto = p.id_producto " +
                                     "WHERE ip.id_inventario = @id", conn);
                cmd.Parameters.AddWithValue("@id", idInventario);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new InventarioProducto
                    {
                        IdInventario = (int)reader["id_inventario"],
                        IdProducto = (int)reader["id_producto"],
                        NombreProducto = reader["nombre_producto"].ToString() ?? "",
                        Cantidad = (int)reader["cantidad"]
                    });
                }
            }

            return lista;
        }

        public static bool Agregar(InventarioProducto ip)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            SqlCommand cmd = new("INSERT INTO InventarioProducto (id_inventario, id_producto, cantidad) " +
                                 "VALUES (@inv, @prod, @cant)", conn);
            cmd.Parameters.AddWithValue("@inv", ip.IdInventario);
            cmd.Parameters.AddWithValue("@prod", ip.IdProducto);
            cmd.Parameters.AddWithValue("@cant", ip.Cantidad);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool Actualizar(InventarioProducto ip)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            SqlCommand cmd = new("UPDATE InventarioProducto SET cantidad = @cant " +
                                 "WHERE id_inventario = @inv AND id_producto = @prod", conn);
            cmd.Parameters.AddWithValue("@inv", ip.IdInventario);
            cmd.Parameters.AddWithValue("@prod", ip.IdProducto);
            cmd.Parameters.AddWithValue("@cant", ip.Cantidad);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool Eliminar(int idInventario, int idProducto)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            SqlCommand cmd = new("DELETE FROM InventarioProducto WHERE id_inventario = @inv AND id_producto = @prod", conn);
            cmd.Parameters.AddWithValue("@inv", idInventario);
            cmd.Parameters.AddWithValue("@prod", idProducto);

            return cmd.ExecuteNonQuery() > 0;
        }
        public static bool EliminarPorProducto(int idProducto)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            SqlCommand cmd = new("DELETE FROM InventarioProducto WHERE id_producto = @idProducto", conn);
            cmd.Parameters.AddWithValue("@idProducto", idProducto);

            return cmd.ExecuteNonQuery() > 0;
        }
        public static bool ActualizarStock(int idProducto, int idInventario, int nuevoStock)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"
            UPDATE InventarioProducto
            SET cantidad = @cantidad
            WHERE id_producto = @idProducto AND id_inventario = @idInventario";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@cantidad", nuevoStock);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);
                cmd.Parameters.AddWithValue("@idInventario", idInventario);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.ActualizarStock] {ex.Message}");
                return false;
            }
        }
        public static List<InventarioProductoView> ObtenerProductosPorUbicacionDeProducto(int idProducto)
        {
            List<InventarioProductoView> lista = [];

            using (var conn = DAOHelper.AbrirConexionSegura())
            {
                SqlCommand cmd = new(@"SELECT ip.id_inventario, ip.id_producto, ip.cantidad, i.descripcion 
                               FROM InventarioProducto ip
                               JOIN Inventario i ON ip.id_inventario = i.id_inventario
                               WHERE ip.id_producto = @id", conn);
                cmd.Parameters.AddWithValue("@id", idProducto);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new InventarioProductoView
                    {
                        IdInventario = (int)reader["id_inventario"],
                        IdProducto = (int)reader["id_producto"],
                        Cantidad = (int)reader["cantidad"],
                        DescripcionUbicacion = reader["descripcion"].ToString() ?? ""
                    });
                }
            }

            return lista;
        }




    }
}
