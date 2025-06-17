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

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"SELECT ip.id_inventario, ip.id_producto, ip.cantidad, i.descripcion
                                 FROM InventarioProducto ip
                                 JOIN Inventario i ON ip.id_inventario = i.id_inventario
                                 WHERE ip.id_producto = @idProducto";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);

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
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"UPDATE InventarioProducto
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
    }
}
