using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class InventarioDAO
    {
        public static List<Inventario> ObtenerInventarios()
        {
            var inventarios = new List<Inventario>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = "SELECT id_inventario, descripcion FROM Inventario";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                int idIndex = reader.GetOrdinal("id_inventario");
                int descIndex = reader.GetOrdinal("descripcion");

                while (reader.Read())
                {
                    inventarios.Add(new Inventario
                    {
                        IdInventario = reader.GetInt32(idIndex),
                        Descripcion = reader.GetString(descIndex)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioDAO.ObtenerInventarios] {ex.Message}");
            }

            return inventarios;
        }

        public static string ObtenerDescripcionPorId(int idInventario)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = "SELECT descripcion FROM Inventario WHERE id_inventario = @id";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idInventario);
                var result = cmd.ExecuteScalar();

                return result?.ToString() ?? "(Desconocido)";
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][InventarioDAO.ObtenerDescripcionPorId] {ex.Message}");
                return "(Error)";
            }
        }

        public static Inventario? ObtenerUltimo()
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = "SELECT TOP 1 id_inventario, descripcion FROM Inventario ORDER BY id_inventario DESC";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                int idIndex = reader.GetOrdinal("id_inventario");
                int descIndex = reader.GetOrdinal("descripcion");

                if (reader.Read())
                {
                    return new Inventario
                    {
                        IdInventario = reader.GetInt32(idIndex),
                        Descripcion = reader.GetString(descIndex)
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
            if (string.IsNullOrWhiteSpace(inventario.Descripcion))
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = @"INSERT INTO Inventario (descripcion) VALUES (@descripcion)";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@descripcion", inventario.Descripcion);

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
            if (inventario.IdInventario <= 0 || string.IsNullOrWhiteSpace(inventario.Descripcion))
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = @"
            UPDATE Inventario 
            SET descripcion = @descripcion 
            WHERE id_inventario = @id";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@descripcion", inventario.Descripcion);
                cmd.Parameters.AddWithValue("@id", inventario.IdInventario);

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
            if (idInventario <= 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                const string query = "DELETE FROM Inventario WHERE id_inventario = @id";
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