using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class InventarioProductoDAO
    {
        public static List<InventarioProducto> ObtenerUbicacionesPorProducto(int idProducto)
        {
            var lista = new List<InventarioProducto>();

            if (idProducto <= 0)
                return lista;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = @"
                SELECT ip.id_inventario, ip.id_producto, ip.cantidad, i.descripcion
                FROM InventarioProducto ip
                JOIN Inventario i ON ip.id_inventario = i.id_inventario
                WHERE ip.id_producto = @idProducto
                ORDER BY ip.id_inventario ASC";  // FIFO: primero los inventarios más antiguos

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);

                using var reader = cmd.ExecuteReader();

                int idInventarioIndex = reader.GetOrdinal("id_inventario");
                int idProductoIndex = reader.GetOrdinal("id_producto");
                int cantidadIndex = reader.GetOrdinal("cantidad");
                int descripcionIndex = reader.GetOrdinal("descripcion");

                while (reader.Read())
                {
                    lista.Add(new InventarioProducto
                    {
                        IdInventario = reader.GetInt32(idInventarioIndex),
                        IdProducto = reader.GetInt32(idProductoIndex),
                        Cantidad = reader.GetInt32(cantidadIndex),
                        DescripcionUbicacion = reader.GetString(descripcionIndex)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.ObtenerUbicacionesPorProducto] {ex.Message}");
            }

            return lista;
        }

        public static List<InventarioProducto> ObtenerPorInventario(int idInventario)
        {
            var lista = new List<InventarioProducto>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"SELECT ip.id_inventario, ip.id_producto, ip.cantidad, p.nombre_producto
                                 FROM InventarioProducto ip
                                 JOIN Producto p ON ip.id_producto = p.id_producto
                                 WHERE ip.id_inventario = @idInventario";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idInventario", idInventario);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new InventarioProducto
                    {
                        IdInventario = reader.GetInt32(0),
                        IdProducto = reader.GetInt32(1),
                        Cantidad = reader.GetInt32(2),
                        DescripcionUbicacion = reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.ObtenerPorInventario] {ex.Message}");
            }

            return lista;
        }

        public static bool Insertar(InventarioProducto item)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"INSERT INTO InventarioProducto (id_inventario, id_producto, cantidad)
                                 VALUES (@idInventario, @idProducto, @cantidad)";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idInventario", item.IdInventario);
                cmd.Parameters.AddWithValue("@idProducto", item.IdProducto);
                cmd.Parameters.AddWithValue("@cantidad", item.Cantidad);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.Insertar] {ex.Message}");
                return false;
            }
        }

        public static bool Actualizar(InventarioProducto item)
        {
            if (item.IdInventario <= 0 || item.IdProducto <= 0 || item.Cantidad < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = @"
            UPDATE InventarioProducto
            SET cantidad = @cantidad
            WHERE id_inventario = @idInventario AND id_producto = @idProducto";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idInventario", item.IdInventario);
                cmd.Parameters.AddWithValue("@idProducto", item.IdProducto);
                cmd.Parameters.AddWithValue("@cantidad", item.Cantidad);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.Actualizar] {ex.Message}");
                return false;
            }
        }

        public static bool Eliminar(int idInventario, int idProducto)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"DELETE FROM InventarioProducto
                                 WHERE id_inventario = @idInventario AND id_producto = @idProducto";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idInventario", idInventario);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.Eliminar] {ex.Message}");
                return false;
            }
        }
        public static List<InventarioProducto> ObtenerInventariosConStock(SqlConnection conn, SqlTransaction trans, int idProducto)
        {
            var lista = new List<InventarioProducto>();

            if (idProducto <= 0)
                return lista;

            try
            {
                const string query = @"
            SELECT ip.id_inventario, ip.id_producto, ip.cantidad
            FROM InventarioProducto ip
            WHERE ip.id_producto = @id AND ip.cantidad > 0
            ORDER BY ip.cantidad DESC";

                using var cmd = new SqlCommand(query, conn, trans);
                cmd.Parameters.AddWithValue("@id", idProducto);

                using var reader = cmd.ExecuteReader();

                int idInvIndex = reader.GetOrdinal("id_inventario");
                int idProdIndex = reader.GetOrdinal("id_producto");
                int cantidadIndex = reader.GetOrdinal("cantidad");

                while (reader.Read())
                {
                    lista.Add(new InventarioProducto
                    {
                        IdInventario = reader.GetInt32(idInvIndex),
                        IdProducto = reader.GetInt32(idProdIndex),
                        Cantidad = reader.GetInt32(cantidadIndex)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.ObtenerInventariosConStock] {ex.Message}");
            }

            return lista;
        }

        // Uso externo (sin transacción)
        public static int ObtenerStockTotal(int idProducto)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                return ObtenerStockTotal(conn, null, idProducto);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.ObtenerStockTotal (externo)] {ex.Message}");
                return 0;
            }
        }

        // Uso interno (con transacción opcional)
        public static int ObtenerStockTotal(SqlConnection conn, SqlTransaction? trans, int idProducto)
        {
            if (idProducto <= 0)
                return 0;

            try
            {
                const string query = "SELECT SUM(cantidad) FROM InventarioProducto WHERE id_producto = @id";

                using var cmd = trans == null
                    ? new SqlCommand(query, conn)
                    : new SqlCommand(query, conn, trans);

                cmd.Parameters.AddWithValue("@id", idProducto);

                var result = cmd.ExecuteScalar();
                return result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.ObtenerStockTotal (interno)] {ex.Message}");
                return 0;
            }
        }


        public static bool InsertarOIncrementar(InventarioProducto item)
        {
            if (item.IdInventario <= 0 || item.IdProducto <= 0 || item.Cantidad <= 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                const string query = @"
            IF EXISTS (SELECT 1 FROM InventarioProducto WHERE id_inventario = @inv AND id_producto = @prod)
                UPDATE InventarioProducto 
                SET cantidad = cantidad + @cant
                WHERE id_inventario = @inv AND id_producto = @prod
            ELSE
                INSERT INTO InventarioProducto (id_inventario, id_producto, cantidad)
                VALUES (@inv, @prod, @cant)";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@inv", item.IdInventario);
                cmd.Parameters.AddWithValue("@prod", item.IdProducto);
                cmd.Parameters.AddWithValue("@cant", item.Cantidad);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioProductoDAO.InsertarOIncrementar] {ex.Message}");
                return false;
            }
        }
    }
}
