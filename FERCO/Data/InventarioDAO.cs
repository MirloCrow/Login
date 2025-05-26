using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class InventarioDAO
    {
        public static List<Inventario> ObtenerInventario()
        {
            var inventarios = new List<Inventario>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT id_inventario, id_producto, cantidad_producto FROM Inventario";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    inventarios.Add(new Inventario
                    {
                        IdInventario = (int)reader["id_inventario"],
                        IdProducto = (int)reader["id_producto"],
                        CantidadProducto = (int)reader["cantidad_producto"],
                        Nombre = $"Inventario ID {reader["id_inventario"]} (Stock: {reader["cantidad_producto"]})"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioDAO.ObtenerInventario] {ex.Message}");
            }

            return inventarios;
        }

        public static Inventario? ObtenerUltimo()
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT TOP 1 * FROM Inventario ORDER BY id_inventario DESC";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Inventario
                    {
                        IdInventario = (int)reader["id_inventario"],
                        IdProducto = (int)reader["id_producto"],
                        CantidadProducto = (int)reader["cantidad_producto"],
                        Nombre = $"Stock: {reader["cantidad_producto"]} (ID {reader["id_inventario"]})"
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioDAO.ObtenerUltimo] {ex.Message}");
            }

            return null;
        }

        public static bool Agregar(Inventario inventario)
        {
            if (inventario.IdProducto <= 0 || inventario.CantidadProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"INSERT INTO Inventario (id_producto, cantidad_producto)
                                 VALUES (@id_producto, @cantidad)";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id_producto", inventario.IdProducto);
                cmd.Parameters.AddWithValue("@cantidad", inventario.CantidadProducto);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioDAO.Agregar] {ex.Message}");
                return false;
            }
        }

        public static bool Actualizar(Inventario inventario)
        {
            if (inventario.IdInventario <= 0 || inventario.IdProducto <= 0 || inventario.CantidadProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"UPDATE Inventario 
                                 SET id_producto = @id_producto, 
                                     cantidad_producto = @cantidad 
                                 WHERE id_inventario = @id_inventario";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id_inventario", inventario.IdInventario);
                cmd.Parameters.AddWithValue("@id_producto", inventario.IdProducto);
                cmd.Parameters.AddWithValue("@cantidad", inventario.CantidadProducto);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioDAO.Actualizar] {ex.Message}");
                return false;
            }
        }

        public static bool Eliminar(int idInventario)
        {
            if (idInventario <= 0) return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "DELETE FROM Inventario WHERE id_inventario = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idInventario);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioDAO.Eliminar] {ex.Message}");
                return false;
            }
        }
    }
}
