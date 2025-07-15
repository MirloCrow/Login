using FERCO.Model;
using Microsoft.Data.SqlClient;
using System;
using System.Windows;

namespace FERCO.Data
{
    public static class VentaDAO
    {
        public static int RegistrarVenta(Venta venta)
        {
            try
            {
                using SqlConnection conn = ConexionBD.ObtenerConexion();
                conn.Open();

                string query = "INSERT INTO Ventas (id_cliente, fecha_venta, total_venta) " +
                               "OUTPUT INSERTED.id_venta VALUES (@cliente, @fecha, @total)";

                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@cliente", venta.IdCliente);
                cmd.Parameters.AddWithValue("@fecha", venta.FechaVenta);
                cmd.Parameters.AddWithValue("@total", venta.TotalVenta);

                int idGenerado = (int)cmd.ExecuteScalar();
                venta.IdVenta = idGenerado; // ← ASIGNACIÓN NECESARIA

                return idGenerado;
            }
            catch (SqlException ex) when (ex.Message.Contains("FOREIGN KEY constraint") && ex.Message.Contains("Cliente"))
            {
                MessageBox.Show("Error: el cliente seleccionado no existe. Por favor, agregue un cliente antes de registrar una venta.",
                                "Cliente no encontrado",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error al registrar la venta:\n" + ex.Message,
                                "Error inesperado",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return -1;
            }
        }

        public static void RegistrarDetalle(DetalleVenta detalle)
        {
            using SqlConnection conn = ConexionBD.ObtenerConexion();
            conn.Open();
            string query = "INSERT INTO Detalle_Venta (id_venta, id_producto, precio_unitario, cantidad_detalle, subtotal_detalle) " +
                           "VALUES (@venta, @producto, @precio, @cantidad, @subtotal)";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@venta", detalle.IdVenta);
            cmd.Parameters.AddWithValue("@producto", detalle.IdProducto);
            cmd.Parameters.AddWithValue("@precio", detalle.PrecioUnitario);
            cmd.Parameters.AddWithValue("@cantidad", detalle.CantidadDetalle);
            cmd.Parameters.AddWithValue("@subtotal", detalle.SubtotalDetalle);

            cmd.ExecuteNonQuery();
        }
        public static List<Venta> ObtenerPorCliente(int idCliente)
        {
            List<Venta> lista = [];

            using var conn = ConexionBD.ObtenerConexion();
            conn.Open();

            var cmd = new SqlCommand(@"
                SELECT v.*, c.nombre_cliente 
                FROM Ventas v
                JOIN Cliente c ON v.id_cliente = c.id_cliente
                WHERE v.id_cliente = @id
                ORDER BY v.fecha_venta DESC", conn);

            cmd.Parameters.AddWithValue("@id", idCliente);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Venta
                {
                    IdVenta = (int)reader["id_venta"],
                    IdCliente = (int)reader["id_cliente"],
                    FechaVenta = (DateTime)reader["fecha_venta"],
                    TotalVenta = (int)reader["total_venta"],
                    ClienteNombre = reader["nombre_cliente"].ToString() ?? ""
                });
            }

            return lista;
        }
        public static List<DetalleVenta> ObtenerDetalles(int idVenta)
        {
            List<DetalleVenta> lista = [];

            using var conn = DAOHelper.AbrirConexionSegura();
            var cmd = new SqlCommand(@"
            SELECT dv.*, p.nombre_producto
            FROM Detalle_Venta dv
            JOIN Producto p ON dv.id_producto = p.id_producto
            WHERE dv.id_venta = @id", conn);

            cmd.Parameters.AddWithValue("@id", idVenta);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new DetalleVenta
                {
                    IdDetalleVenta = (int)reader["id_detalle_venta"],
                    IdVenta = (int)reader["id_venta"],
                    IdProducto = (int)reader["id_producto"],
                    CantidadDetalle = (int)reader["cantidad_detalle"],
                    PrecioUnitario = (int)Convert.ToDecimal(reader["precio_unitario"]),
                    NombreProducto = reader["nombre_producto"].ToString() ?? ""
                });
            }

            return lista;
        }

    }
}