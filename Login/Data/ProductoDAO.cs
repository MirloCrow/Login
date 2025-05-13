using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Login.Model;

namespace Login.Data
{
    public static class ProductoDAO
    {
        public static List<Producto> ObtenerTodos()
        {
            List<Producto> lista = new();

            using SqlConnection conn = ConexionBD.ObtenerConexion();
            string query = "SELECT id_producto, nombre_producto, precio_producto, stock_producto FROM Producto";

            using SqlCommand cmd = new(query, conn);
            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Producto
                {
                    IdProducto = reader.GetInt32(0),
                    NombreProducto = reader.GetString(1),
                    PrecioProducto = reader.GetInt32(2),
                    StockProducto = reader.GetInt32(3)
                });
            }

            return lista;
        }

        public static bool ActualizarStock(int idProducto, int nuevoStock)
        {
            using SqlConnection conn = ConexionBD.ObtenerConexion();
            string query = "UPDATE Producto SET stock_producto = @stock WHERE id_producto = @id";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@stock", nuevoStock);
            cmd.Parameters.AddWithValue("@id", idProducto);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static int ObtenerStockPorId(int idProducto)
        {
            using SqlConnection conn = ConexionBD.ObtenerConexion();
            string query = "SELECT stock_producto FROM Producto WHERE id_producto = @id";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@id", idProducto);

            object result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

    }
}
