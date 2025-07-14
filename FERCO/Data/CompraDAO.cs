using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public class CompraDAO
    {
        public static int RegistrarPedido(Pedido pedido)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            using var tran = conn.BeginTransaction();

            try
            {
                // Insertar Pedido
                var cmdPedido = new SqlCommand(
                    "INSERT INTO Pedido_Proveedor (id_proveedor, fecha_pedido, total_pedido) " +
                    "OUTPUT INSERTED.id_pedido VALUES (@prov, @fecha, @total)", conn, tran);
                cmdPedido.Parameters.AddWithValue("@prov", pedido.IdProveedor);
                cmdPedido.Parameters.AddWithValue("@fecha", pedido.FechaPedido);
                cmdPedido.Parameters.AddWithValue("@total", pedido.TotalPedido);

                int idPedido = DAOHelper.EjecutarEscalar(cmdPedido);

                // Insertar Detalles
                foreach (var det in pedido.Detalles)
                {
                    var cmdDetalle = new SqlCommand(
                        "INSERT INTO Detalle_Pedido (id_pedido, id_producto, precio_unitario, cantidad_detalle, subtotal_detalle) " +
                        "VALUES (@idPedido, @idProd, @precio, @cant, @subtotal)", conn, tran);
                    cmdDetalle.Parameters.AddWithValue("@idPedido", idPedido);
                    cmdDetalle.Parameters.AddWithValue("@idProd", det.IdProducto);
                    cmdDetalle.Parameters.AddWithValue("@precio", det.PrecioUnitario);
                    cmdDetalle.Parameters.AddWithValue("@cant", det.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@subtotal", det.Subtotal);
                    cmdDetalle.Transaction = tran;

                    DAOHelper.EjecutarNoQuery(cmdDetalle);
                }

                tran.Commit();
                return idPedido;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                Console.Error.WriteLine($"[ERROR][CompraDAO.RegistrarPedido] {ex.Message}");
                throw;
            }
        }

        public static List<Pedido> ObtenerPedidosConDetalles()
        {
            List<Pedido> pedidos = [];

            using var conn = DAOHelper.AbrirConexionSegura();
            string query = @"
            SELECT p.id_pedido, p.id_proveedor, p.fecha_pedido, p.total_pedido,
                   pr.nombre_proveedor
            FROM Pedido_Proveedor p
            JOIN Proveedor pr ON p.id_proveedor = pr.id_proveedor
            ORDER BY p.fecha_pedido DESC";

            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                pedidos.Add(new Pedido
                {
                    IdPedido = reader.GetInt32(0),
                    IdProveedor = reader.GetInt32(1),
                    FechaPedido = reader.GetDateTime(2),
                    TotalPedido = reader.GetInt32(3),
                    NombreProveedor = reader.GetString(4)
                });
            }

            foreach (var pedido in pedidos)
            {
                pedido.Detalles = ObtenerDetallesPorPedido(pedido.IdPedido);
            }

            return pedidos;
        }

        private static List<DetallePedido> ObtenerDetallesPorPedido(int idPedido)
        {
            List<DetallePedido> detalles = [];

            using var conn = DAOHelper.AbrirConexionSegura();
            string query = @"
            SELECT d.id_producto, p.nombre_producto, d.cantidad_detalle, d.precio_unitario
            FROM Detalle_Pedido d
            JOIN Producto p ON d.id_producto = p.id_producto
            WHERE d.id_pedido = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", idPedido);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                detalles.Add(new DetallePedido
                {
                    IdProducto = reader.GetInt32(0),
                    NombreProducto = reader.GetString(1),
                    Cantidad = reader.GetInt32(2),
                    PrecioUnitario = reader.GetInt32(3)
                });
            }

            return detalles;
        }

    }
}
