using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Login.Model;

namespace Login.Data
{
    public class InventarioDAO
    {
        public static List<Inventario> ObtenerInventario()
        {
            var lista = new List<Inventario>();
            using var con = ConexionBD.ObtenerConexion();
            var cmd = new SqlCommand(@"
            SELECT i.id_inventario, i.id_producto, p.nombre_producto, i.cantidad_producto
            FROM Inventario i
            INNER JOIN Producto p ON i.id_producto = p.id_producto", con);
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Inventario
                {
                    IdInventario = (int)reader["id_inventario"],
                    IdProducto = (int)reader["id_producto"],
                    NombreProducto = reader["nombre_producto"]?.ToString() ?? string.Empty,
                    CantidadProducto = (int)reader["cantidad_producto"]
                });
            }
            return lista;
        }

        public static void Agregar(Inventario inv)
        {
            using var con = ConexionBD.ObtenerConexion();
            var cmd = new SqlCommand("INSERT INTO Inventario (id_inventario, id_producto, cantidad_producto) VALUES (@id, @prod, @cant)", con);
            cmd.Parameters.AddWithValue("@id", inv.IdInventario);
            cmd.Parameters.AddWithValue("@prod", inv.IdProducto);
            cmd.Parameters.AddWithValue("@cant", inv.CantidadProducto);
            cmd.ExecuteNonQuery();
        }

        public static void Actualizar(Inventario inv)
        {
            using var con = ConexionBD.ObtenerConexion();
            var cmd = new SqlCommand("UPDATE Inventario SET id_producto = @prod, cantidad_producto = @cant WHERE id_inventario = @id", con);
            cmd.Parameters.AddWithValue("@id", inv.IdInventario);
            cmd.Parameters.AddWithValue("@prod", inv.IdProducto);
            cmd.Parameters.AddWithValue("@cant", inv.CantidadProducto);
            cmd.ExecuteNonQuery();
        }

        public static void Eliminar(int id)
        {
            using var con = ConexionBD.ObtenerConexion();
            var cmd = new SqlCommand("DELETE FROM Inventario WHERE id_inventario = @id", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
