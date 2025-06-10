using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class InventarioDAO
    {
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

        public static List<UbicacionInventario> ObtenerUbicacionesConProductos()
        {
            List<UbicacionInventario> lista = new();

            using var conn = DAOHelper.AbrirConexionSegura();
            var cmd = new SqlCommand(@"
                SELECT i.id_inventario, i.descripcion, 
                       ISNULL(SUM(ip.cantidad), 0) AS TotalProductos
                FROM Inventario i
                LEFT JOIN InventarioProducto ip ON i.id_inventario = ip.id_inventario
                GROUP BY i.id_inventario, i.descripcion", conn);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new UbicacionInventario
                {
                    IdInventario = (int)reader["id_inventario"],
                    Descripcion = reader["descripcion"].ToString() ?? "",
                    TotalProductos = (int)reader["TotalProductos"]
                });
            }

            return lista;
        }

        public static bool AgregarUbicacion(UbicacionInventario ubicacion)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            var cmd = new SqlCommand("INSERT INTO Inventario (descripcion) VALUES (@desc)", conn);
            cmd.Parameters.AddWithValue("@desc", ubicacion.Descripcion);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool ActualizarUbicacion(UbicacionInventario ubicacion)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            var cmd = new SqlCommand("UPDATE Inventario SET descripcion = @desc WHERE id_inventario = @id", conn);
            cmd.Parameters.AddWithValue("@desc", ubicacion.Descripcion);
            cmd.Parameters.AddWithValue("@id", ubicacion.IdInventario);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool EliminarUbicacion(int idInventario)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            var cmd = new SqlCommand("DELETE FROM Inventario WHERE id_inventario = @id", conn);
            cmd.Parameters.AddWithValue("@id", idInventario);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static List<InventarioProducto> ObtenerProductosPorUbicacion(int idInventario)
        {
            return InventarioProductoDAO.ObtenerProductosPorUbicacion(idInventario);
        }
        public static string ObtenerDescripcionUbicacion(int idInventario)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            string query = "SELECT descripcion FROM Inventario WHERE id_inventario = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idInventario);

            object result = cmd.ExecuteScalar();
            return result?.ToString() ?? "Ubicación desconocida";
        }

    }
}
