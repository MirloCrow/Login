using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Login.Model;

namespace Login.Data
{
    public static class ProveedorDAO
    {
        public static List<Proveedor> ObtenerTodos()
        {
            var proveedores = new List<Proveedor>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT id_proveedor, nombre_proveedor, email_proveedor, direccion_proveedor, telefono_proveedor FROM Proveedor";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    proveedores.Add(new Proveedor
                    {
                        IdProveedor = (int)reader["id_proveedor"],
                        Nombre = reader["nombre_proveedor"].ToString() ?? "",
                        Email = reader["email_proveedor"].ToString() ?? "",
                        Direccion = reader["direccion_proveedor"].ToString() ?? "",
                        Telefono = (int)reader["telefono_proveedor"]
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProveedorDAO.ObtenerTodos] {ex.Message}");
            }

            return proveedores;
        }

        public static bool Agregar(Proveedor proveedor)
        {
            if (string.IsNullOrWhiteSpace(proveedor.Nombre))
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"INSERT INTO Proveedor (nombre_proveedor, email_proveedor, direccion_proveedor, telefono_proveedor)
                                 VALUES (@nombre, @email, @direccion, @telefono)";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombre", proveedor.Nombre);
                cmd.Parameters.AddWithValue("@email", proveedor.Email ?? "");
                cmd.Parameters.AddWithValue("@direccion", proveedor.Direccion ?? "");
                cmd.Parameters.AddWithValue("@telefono", proveedor.Telefono);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProveedorDAO.Agregar] {ex.Message}");
                return false;
            }
        }

        public static bool Actualizar(Proveedor proveedor)
        {
            if (proveedor.IdProveedor <= 0 || string.IsNullOrWhiteSpace(proveedor.Nombre))
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"UPDATE Proveedor 
                                 SET nombre_proveedor = @nombre,
                                     email_proveedor = @correo,
                                     direccion_proveedor = @direccion,
                                     telefono_proveedor = @telefono
                                 WHERE id_proveedor = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@nombre", proveedor.Nombre);
                cmd.Parameters.AddWithValue("@correo", proveedor.Email ?? "");
                cmd.Parameters.AddWithValue("@direccion", proveedor.Direccion ?? "");
                cmd.Parameters.AddWithValue("@telefono", proveedor.Telefono);
                cmd.Parameters.AddWithValue("@id", proveedor.IdProveedor);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProveedorDAO.Actualizar] {ex.Message}");
                return false;
            }
        }

        public static bool Eliminar(int idProveedor)
        {
            if (idProveedor <= 0) return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "DELETE FROM Proveedor WHERE id_proveedor = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idProveedor);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProveedorDAO.Eliminar] {ex.Message}");
                return false;
            }
        }

        public static Proveedor? ObtenerUltimo()
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT TOP 1 * FROM Proveedor ORDER BY id_proveedor DESC";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new Proveedor
                    {
                        IdProveedor = (int)reader["id_proveedor"],
                        Nombre = reader["nombre_proveedor"].ToString() ?? "",
                        Email = reader["email_proveedor"].ToString() ?? "",
                        Direccion = reader["direccion_proveedor"].ToString() ?? "",
                        Telefono = (int)reader["telefono_proveedor"]
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProveedorDAO.ObtenerUltimo] {ex.Message}");
            }

            return null;
        }
    }
}
