using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class CategoriaDAO
    {
        public static List<Categoria> ObtenerTodas()
        {
            var categorias = new List<Categoria>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT id_categoria, nombre_categoria, descripcion_categoria FROM Categoria";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    categorias.Add(new Categoria
                    {
                        IdCategoria = (int)reader["id_categoria"],
                        Nombre = reader["nombre_categoria"].ToString() ?? "",
                        Descripcion = reader["descripcion_categoria"].ToString() ?? ""
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][CategoriaDAO.ObtenerTodas] {ex.Message}");
            }

            return categorias;
        }

        public static bool Agregar(Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"INSERT INTO Categoria (nombre_categoria, descripcion_categoria)
                                 VALUES (@nombre, @descripcion)";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", categoria.Descripcion ?? "");

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][CategoriaDAO.Agregar] {ex.Message}");
                return false;
            }
        }

        public static Categoria? ObtenerUltima()
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT TOP 1 * FROM Categoria ORDER BY id_categoria DESC";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Categoria
                    {
                        IdCategoria = (int)reader["id_categoria"],
                        Nombre = reader["nombre_categoria"].ToString() ?? "",
                        Descripcion = reader["descripcion_categoria"].ToString() ?? ""
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][CategoriaDAO.ObtenerUltima] {ex.Message}");
            }

            return null;
        }
        public static bool Actualizar(Categoria categoria)
        {
            if (categoria.IdCategoria <= 0 || string.IsNullOrWhiteSpace(categoria.Nombre))
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"UPDATE Categoria 
                         SET nombre_categoria = @nombre, descripcion_categoria = @descripcion
                         WHERE id_categoria = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", categoria.IdCategoria);
                cmd.Parameters.AddWithValue("@nombre", categoria.Nombre);
                cmd.Parameters.AddWithValue("@descripcion", categoria.Descripcion);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][CategoriaDAO.Actualizar] {ex.Message}");
                return false;
            }
        }

        public static bool Eliminar(int idCategoria)
        {
            if (idCategoria <= 0) return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "DELETE FROM Categoria WHERE id_categoria = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idCategoria);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][CategoriaDAO.Eliminar] {ex.Message}");
                return false;
            }
        }

    }
}
