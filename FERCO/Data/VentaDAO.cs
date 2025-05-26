using System;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class VentaDAO
    {
        public static int RegistrarVenta(Venta venta)
        {
            using SqlConnection conn = ConexionBD.ObtenerConexion();
            conn.Open();
            string query = "INSERT INTO Ventas (id_cliente, fecha_venta, total_venta) OUTPUT INSERTED.id_venta VALUES (@cliente, @fecha, @total)";
            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@cliente", venta.IdCliente);
            cmd.Parameters.AddWithValue("@fecha", venta.FechaVenta);
            cmd.Parameters.AddWithValue("@total", venta.TotalVenta);

            return (int)cmd.ExecuteScalar(); // devuelve el ID generado
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
    }
}