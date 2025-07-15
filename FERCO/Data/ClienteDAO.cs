using FERCO.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace FERCO.Data
{
    public static class ClienteDAO
    {
        public static Cliente? BuscarPorNombre(string nombre)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                using var cmd = new SqlCommand("SELECT TOP 1 * FROM Cliente WHERE nombre_cliente = @nombre", conn);
                cmd.Parameters.AddWithValue("@nombre", nombre);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Cliente
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                        RutCliente = reader.GetInt32(reader.GetOrdinal("rut_cliente")),
                        EstadoCliente = reader.GetBoolean(reader.GetOrdinal("estado_cliente")),
                        NombreCliente = reader.GetString(reader.GetOrdinal("nombre_cliente")),
                        EmailCliente = reader.GetString(reader.GetOrdinal("email_cliente")),
                        DireccionCliente = reader.GetString(reader.GetOrdinal("direccion_cliente")),
                        TelefonoCliente = reader.GetInt32(reader.GetOrdinal("telefono_cliente"))
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ClienteDAO.BuscarPorNombre] {ex.Message}");
            }

            return null;
        }

        public static bool Actualizar(Cliente cliente)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                using var cmd = new SqlCommand(
                    "UPDATE Cliente SET nombre_cliente = @nombre, email_cliente = @email, direccion_cliente = @direccion, " +
                    "telefono_cliente = @telefono, estado_cliente = @estado WHERE id_cliente = @id", conn);

                cmd.Parameters.AddWithValue("@nombre", cliente.NombreCliente);
                cmd.Parameters.AddWithValue("@email", cliente.EmailCliente);
                cmd.Parameters.AddWithValue("@direccion", cliente.DireccionCliente);
                cmd.Parameters.AddWithValue("@telefono", cliente.TelefonoCliente);
                cmd.Parameters.AddWithValue("@estado", cliente.EstadoCliente);
                cmd.Parameters.AddWithValue("@id", cliente.IdCliente);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ClienteDAO.Actualizar] {ex.Message}");
                return false;
            }
        }

        public static Cliente? ObtenerPorId(int idCliente)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                using var cmd = new SqlCommand("SELECT * FROM Cliente WHERE id_cliente = @id", conn);
                cmd.Parameters.AddWithValue("@id", idCliente);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Cliente
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                        RutCliente = reader.GetInt32(reader.GetOrdinal("rut_cliente")),
                        EstadoCliente = reader.GetBoolean(reader.GetOrdinal("estado_cliente")),
                        NombreCliente = reader.GetString(reader.GetOrdinal("nombre_cliente")),
                        EmailCliente = reader.GetString(reader.GetOrdinal("email_cliente")),
                        DireccionCliente = reader.GetString(reader.GetOrdinal("direccion_cliente")),
                        TelefonoCliente = reader.GetInt32(reader.GetOrdinal("telefono_cliente"))
                    };
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ClienteDAO.ObtenerPorId] {ex.Message}");
            }

            return null;
        }

        public static int AgregarYObtenerId(Cliente cliente)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                using var cmd = new SqlCommand(
                    "INSERT INTO Cliente (rut_cliente, estado_cliente, nombre_cliente, email_cliente, direccion_cliente, telefono_cliente) " +
                    "OUTPUT INSERTED.id_cliente " +
                    "VALUES (@rut, @estado, @nombre, @email, @direccion, @telefono)", conn);

                cmd.Parameters.AddWithValue("@rut", cliente.RutCliente);
                cmd.Parameters.AddWithValue("@estado", cliente.EstadoCliente);
                cmd.Parameters.AddWithValue("@nombre", cliente.NombreCliente);
                cmd.Parameters.AddWithValue("@email", cliente.EmailCliente);
                cmd.Parameters.AddWithValue("@direccion", cliente.DireccionCliente);
                cmd.Parameters.AddWithValue("@telefono", cliente.TelefonoCliente);

                return DAOHelper.EjecutarEscalar(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ClienteDAO.AgregarYObtenerId] {ex.Message}");
                return 0;
            }
        }


        public static List<Cliente> ObtenerTodos()
        {
            List<Cliente> lista = [];

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                using var cmd = new SqlCommand("SELECT * FROM Cliente", conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Cliente
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                        RutCliente = reader.GetInt32(reader.GetOrdinal("rut_cliente")),
                        EstadoCliente = reader.GetBoolean(reader.GetOrdinal("estado_cliente")),
                        NombreCliente = reader.GetString(reader.GetOrdinal("nombre_cliente")),
                        EmailCliente = reader.GetString(reader.GetOrdinal("email_cliente")),
                        DireccionCliente = reader.GetString(reader.GetOrdinal("direccion_cliente")),
                        TelefonoCliente = reader.GetInt32(reader.GetOrdinal("telefono_cliente"))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ClienteDAO.ObtenerTodos] {ex.Message}");
            }

            return lista;
        }

        public static List<Cliente> BuscarPorCriterio(string criterio)
        {
            List<Cliente> lista = [];

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                bool esRut = int.TryParse(criterio, out int rut);
                using var cmd = esRut
                    ? new SqlCommand("SELECT * FROM Cliente WHERE rut_cliente = @rut", conn)
                    : new SqlCommand("SELECT * FROM Cliente WHERE nombre_cliente LIKE @nombre", conn);

                if (esRut)
                    cmd.Parameters.AddWithValue("@rut", rut);
                else
                    cmd.Parameters.AddWithValue("@nombre", $"%{criterio}%");

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new Cliente
                    {
                        IdCliente = reader.GetInt32(reader.GetOrdinal("id_cliente")),
                        RutCliente = reader.GetInt32(reader.GetOrdinal("rut_cliente")),
                        EstadoCliente = reader.GetBoolean(reader.GetOrdinal("estado_cliente")),
                        NombreCliente = reader.GetString(reader.GetOrdinal("nombre_cliente")),
                        EmailCliente = reader.GetString(reader.GetOrdinal("email_cliente")),
                        DireccionCliente = reader.GetString(reader.GetOrdinal("direccion_cliente")),
                        TelefonoCliente = reader.GetInt32(reader.GetOrdinal("telefono_cliente"))
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ClienteDAO.BuscarPorCriterio] {ex.Message}");
            }

            return lista;
        }
    }
}
