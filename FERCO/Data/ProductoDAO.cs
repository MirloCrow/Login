﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class ProductoDAO
    {
        // Buscar y obtener productos
        public static List<Producto> ObtenerTodos()
        {
            var productos = new List<Producto>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                string query = @"
            SELECT p.id_producto,
                   p.codigo_producto,
                   p.nombre_producto, 
                   p.descripcion_producto, 
                   p.precio_producto, 
                   p.id_proveedor, 
                   p.id_categoria,
                   pr.nombre_proveedor,
                   c.nombre_categoria
            FROM Producto p
            JOIN Proveedor pr ON p.id_proveedor = pr.id_proveedor
            JOIN Categoria c ON p.id_categoria = c.id_categoria";

                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int idProducto = reader.GetInt32(0);

                    var producto = new Producto
                    {
                        IdProducto = idProducto,
                        CodigoProducto = reader.GetString(1),
                        NombreProducto = reader.GetString(2),
                        DescripcionProducto = reader.GetString(3),
                        PrecioProducto = reader.GetInt32(4),
                        IdProveedor = reader.GetInt32(5),
                        IdCategoria = reader.GetInt32(6),
                        NombreProveedor = reader.GetString(7),
                        NombreCategoria = reader.GetString(8),
                        UbicacionesConStock = InventarioProductoDAO.ObtenerUbicacionesPorProducto(idProducto)
                    };

                    productos.Add(producto);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.ObtenerTodos] {ex.Message}");
            }

            return productos;
        }

        public static Producto? BuscarPorNombre(string nombre)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new("SELECT * FROM Producto WHERE nombre_producto = @nombre", conn);
            cmd.Parameters.AddWithValue("@nombre", nombre);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Producto
                {
                    IdProducto = (int)reader["id_producto"],
                    NombreProducto = reader["nombre_producto"].ToString() ?? "",
                    DescripcionProducto = reader["descripcion_producto"].ToString() ?? "",
                    PrecioProducto = (int)reader["precio_producto"],
                    IdCategoria = (int)reader["id_categoria"],
                    IdProveedor = (int)reader["id_proveedor"]
                };
            }
            return null;
        }

        public static Producto? BuscarPorCodigo(string codigo)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new("SELECT * FROM Producto WHERE codigo_producto = @codigo", conn);
            cmd.Parameters.AddWithValue("@codigo", codigo);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Producto
                {
                    IdProducto = (int)reader["id_producto"],
                    CodigoProducto = reader["codigo_producto"].ToString() ?? "",
                    NombreProducto = reader["nombre_producto"].ToString() ?? "",
                    DescripcionProducto = reader["descripcion_producto"].ToString() ?? "",
                    PrecioProducto = (int)reader["precio_producto"],
                    IdCategoria = (int)reader["id_categoria"],
                    IdProveedor = (int)reader["id_proveedor"]
                };
            }
            return null;
        }

        public static int ObtenerUltimoId()
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT MAX(id_producto) FROM Producto";
                using var cmd = new SqlCommand(query, conn);
                return DAOHelper.EjecutarEscalar(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.ObtenerUltimoId] {ex.Message}");
                return 0;
            }
        }

        // CRUD Producto
        public static bool Agregar(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.NombreProducto) ||
                string.IsNullOrWhiteSpace(producto.CodigoProducto) ||
                producto.PrecioProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"INSERT INTO Producto (id_proveedor, id_categoria, nombre_producto, descripcion_producto, precio_producto, codigo_producto)
                         VALUES (@proveedor, @categoria, @nombre, @descripcion, @precio, @codigoproducto)";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@proveedor", producto.IdProveedor);
                cmd.Parameters.AddWithValue("@categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@nombre", producto.NombreProducto);
                cmd.Parameters.AddWithValue("@descripcion", producto.DescripcionProducto);
                cmd.Parameters.AddWithValue("@precio", producto.PrecioProducto);
                cmd.Parameters.AddWithValue("@codigoproducto", producto.CodigoProducto);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.Agregar] {ex.Message}");
                return false;
            }
        }

        public static bool Actualizar(Producto producto)
        {
            if (producto.IdProducto <= 0 || string.IsNullOrWhiteSpace(producto.NombreProducto) ||
                producto.PrecioProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"UPDATE Producto
                                 SET codigo_producto = @codigo,
                                     id_proveedor = @proveedor,
                                     id_categoria = @categoria,
                                     nombre_producto = @nombre,
                                     descripcion_producto = @descripcion,
                                     precio_producto = @precio
                                 WHERE id_producto = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", producto.IdProducto);
                cmd.Parameters.AddWithValue("@proveedor", producto.IdProveedor);
                cmd.Parameters.AddWithValue("@categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@nombre", producto.NombreProducto);
                cmd.Parameters.AddWithValue("@descripcion", producto.DescripcionProducto);
                cmd.Parameters.AddWithValue("@precio", producto.PrecioProducto);
                cmd.Parameters.AddWithValue("@codigo", producto.CodigoProducto);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.Actualizar] {ex.Message}");
                return false;
            }
        }

        public static bool Eliminar(int idProducto)
        {
            if (idProducto <= 0) return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "DELETE FROM Producto WHERE id_producto = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idProducto);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.Eliminar] {ex.Message}");
                return false;
            }
        }
    }
}
