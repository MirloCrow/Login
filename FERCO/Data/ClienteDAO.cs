﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class ClienteDAO
    {
        public static Cliente? BuscarPorNombre(string nombre)
        {
            using var conn = ConexionBD.ObtenerConexion();
            conn.Open();
            var cmd = new SqlCommand("SELECT TOP 1 * FROM Cliente WHERE nombre_cliente = @nombre", conn);
            cmd.Parameters.AddWithValue("@nombre", nombre);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    IdCliente = (int)reader["id_cliente"],
                    RutCliente = (int)reader["rut_cliente"],
                    EstadoCliente = (bool)reader["estado_cliente"],
                    NombreCliente = reader["nombre_cliente"].ToString()!,
                    EmailCliente = reader["email_cliente"].ToString()!,
                    DireccionCliente = reader["direccion_cliente"].ToString()!,
                    TelefonoCliente = (int)reader["telefono_cliente"]
                };
            }

            return null;
        }

        public static Cliente? ObtenerPorId(int idCliente)
        {
            using var conn = ConexionBD.ObtenerConexion();
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Cliente WHERE id_cliente = @id", conn);
            cmd.Parameters.AddWithValue("@id", idCliente);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Cliente
                {
                    IdCliente = (int)reader["id_cliente"],
                    RutCliente = (int)reader["rut_cliente"],
                    EstadoCliente = (bool)reader["estado_cliente"],
                    NombreCliente = reader["nombre_cliente"].ToString()!,
                    EmailCliente = reader["email_cliente"].ToString()!,
                    DireccionCliente = reader["direccion_cliente"].ToString()!,
                    TelefonoCliente = (int)reader["telefono_cliente"]
                };
            }

            return null;
        }


        public static int AgregarYObtenerId(Cliente cliente)
        {
            using var conn = ConexionBD.ObtenerConexion();
            conn.Open();
            var cmd = new SqlCommand(
                "INSERT INTO Cliente (rut_cliente, estado_cliente, nombre_cliente, email_cliente, direccion_cliente, telefono_cliente) " +
                "OUTPUT INSERTED.id_cliente " +
                "VALUES (@rut, @estado, @nombre, @email, @direccion, @telefono)", conn);

            cmd.Parameters.AddWithValue("@rut", cliente.RutCliente);
            cmd.Parameters.AddWithValue("@estado", cliente.EstadoCliente);
            cmd.Parameters.AddWithValue("@nombre", cliente.NombreCliente);
            cmd.Parameters.AddWithValue("@email", cliente.EmailCliente);
            cmd.Parameters.AddWithValue("@direccion", cliente.DireccionCliente);
            cmd.Parameters.AddWithValue("@telefono", cliente.TelefonoCliente);

            return (int)cmd.ExecuteScalar();
        }

        public static List<Cliente> ObtenerTodos()
        {
            List<Cliente> lista = [];
            using var conn = ConexionBD.ObtenerConexion();
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM Cliente", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Cliente
                {
                    IdCliente = (int)reader["id_cliente"],
                    RutCliente = (int)reader["rut_cliente"],
                    EstadoCliente = (bool)reader["estado_cliente"],
                    NombreCliente = reader["nombre_cliente"].ToString()!,
                    EmailCliente = reader["email_cliente"].ToString()!,
                    DireccionCliente = reader["direccion_cliente"].ToString()!,
                    TelefonoCliente = (int)reader["telefono_cliente"]
                });
            }
            return lista;
        }
    }
}
