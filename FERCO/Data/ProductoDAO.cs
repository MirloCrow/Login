using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class ProductoDAO
    {
        public static List<Producto> ObtenerTodos()
        {
            var productos = new List<Producto>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT id_producto, nombre_producto, precio_producto, stock_producto FROM Producto";
                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    productos.Add(new Producto
                    {
                        IdProducto = reader.GetInt32(0),
                        NombreProducto = reader.GetString(1),
                        PrecioProducto = reader.GetInt32(2),
                        StockProducto = reader.GetInt32(3)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.ObtenerTodos] {ex.Message}");
            }

            return productos;
        }

        public static bool Agregar(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.NombreProducto) || producto.PrecioProducto < 0 || producto.StockProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"INSERT INTO Producto (id_proveedor, id_categoria, nombre_producto, descripcion_producto, stock_producto, precio_producto)
                                 VALUES (@proveedor, @categoria, @nombre, @descripcion, @stock, @precio)";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@proveedor", producto.IdProveedor);
                cmd.Parameters.AddWithValue("@categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@nombre", producto.NombreProducto);
                cmd.Parameters.AddWithValue("@descripcion", producto.DescripcionProducto);
                cmd.Parameters.AddWithValue("@stock", producto.StockProducto);
                cmd.Parameters.AddWithValue("@precio", producto.PrecioProducto);

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
                producto.PrecioProducto < 0 || producto.StockProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"UPDATE Producto
                                 SET id_proveedor = @proveedor,
                                     id_categoria = @categoria,
                                     nombre_producto = @nombre,
                                     descripcion_producto = @descripcion,
                                     stock_producto = @stock,
                                     precio_producto = @precio
                                 WHERE id_producto = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", producto.IdProducto);
                cmd.Parameters.AddWithValue("@proveedor", producto.IdProveedor);
                cmd.Parameters.AddWithValue("@categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@nombre", producto.NombreProducto);
                cmd.Parameters.AddWithValue("@descripcion", producto.DescripcionProducto);
                cmd.Parameters.AddWithValue("@stock", producto.StockProducto);
                cmd.Parameters.AddWithValue("@precio", producto.PrecioProducto);

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

        public static int ObtenerStockPorId(int idProducto)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "SELECT stock_producto FROM Producto WHERE id_producto = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idProducto);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.ObtenerStockPorId] {ex.Message}");
                return 0;
            }
        }

        public static bool ActualizarStock(int idProducto, int nuevoStock)
        {
            if (idProducto <= 0 || nuevoStock < 0) return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = "UPDATE Producto SET stock_producto = @stock WHERE id_producto = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@stock", nuevoStock);
                cmd.Parameters.AddWithValue("@id", idProducto);
                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.ActualizarStock] {ex.Message}");
                return false;
            }
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
    }
}
