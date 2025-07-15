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
                using var cmdPedido = new SqlCommand(@"
            INSERT INTO Pedido_Proveedor (id_proveedor, fecha_pedido, total_pedido) 
            OUTPUT INSERTED.id_pedido 
            VALUES (@prov, @fecha, @total)", conn, tran);

                cmdPedido.Parameters.AddWithValue("@prov", pedido.IdProveedor);
                cmdPedido.Parameters.AddWithValue("@fecha", pedido.FechaPedido);
                cmdPedido.Parameters.AddWithValue("@total", pedido.TotalPedido);

                int idPedido = DAOHelper.EjecutarEscalar(cmdPedido);

                // Insertar Detalles del Pedido
                foreach (var det in pedido.Detalles)
                {
                    using var cmdDetalle = new SqlCommand(@"
                INSERT INTO Detalle_Pedido (id_pedido, id_producto, precio_unitario, cantidad_detalle, subtotal_detalle) 
                VALUES (@idPedido, @idProd, @precio, @cant, @subtotal)", conn, tran);

                    cmdDetalle.Parameters.AddWithValue("@idPedido", idPedido);
                    cmdDetalle.Parameters.AddWithValue("@idProd", det.IdProducto);
                    cmdDetalle.Parameters.AddWithValue("@precio", det.PrecioUnitario);
                    cmdDetalle.Parameters.AddWithValue("@cant", det.Cantidad);
                    cmdDetalle.Parameters.AddWithValue("@subtotal", det.Subtotal);

                    DAOHelper.EjecutarNoQuery(cmdDetalle);
                }

                tran.Commit();
                return idPedido;
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); } catch { /* opcional: log adicional */ }
                Console.Error.WriteLine($"[ERROR][CompraDAO.RegistrarPedido] {ex.Message}");
                return 0; 
            }
        }

        public static List<Pedido> ObtenerPedidosConDetalles()
        {
            List<Pedido> pedidos = [];

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                string query = @"
            SELECT p.id_pedido, p.id_proveedor, p.fecha_pedido, p.total_pedido,
                   pr.nombre_proveedor
            FROM Pedido_Proveedor p
            JOIN Proveedor pr ON p.id_proveedor = pr.id_proveedor
            ORDER BY p.fecha_pedido DESC";

                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                int idPedidoIndex = reader.GetOrdinal("id_pedido");
                int idProveedorIndex = reader.GetOrdinal("id_proveedor");
                int fechaIndex = reader.GetOrdinal("fecha_pedido");
                int totalIndex = reader.GetOrdinal("total_pedido");
                int nombreProveedorIndex = reader.GetOrdinal("nombre_proveedor");

                while (reader.Read())
                {
                    pedidos.Add(new Pedido
                    {
                        IdPedido = reader.GetInt32(idPedidoIndex),
                        IdProveedor = reader.GetInt32(idProveedorIndex),
                        FechaPedido = reader.GetDateTime(fechaIndex),
                        TotalPedido = reader.GetInt32(totalIndex),
                        NombreProveedor = reader.GetString(nombreProveedorIndex)
                    });
                }

                // Obtener detalles asociados a cada pedido
                foreach (var pedido in pedidos)
                {
                    pedido.Detalles = ObtenerDetallesPorPedido(pedido.IdPedido);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][PedidoDAO.ObtenerPedidosConDetalles] {ex.Message}");
            }

            return pedidos;
        }


        private static List<DetallePedido> ObtenerDetallesPorPedido(int idPedido)
        {
            List<DetallePedido> detalles = [];

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();
                string query = @"
            SELECT d.id_producto, p.nombre_producto, d.cantidad_detalle, d.precio_unitario
            FROM Detalle_Pedido d
            JOIN Producto p ON d.id_producto = p.id_producto
            WHERE d.id_pedido = @id";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idPedido);

                using var reader = cmd.ExecuteReader();

                int idProdIndex = reader.GetOrdinal("id_producto");
                int nombreProdIndex = reader.GetOrdinal("nombre_producto");
                int cantidadIndex = reader.GetOrdinal("cantidad_detalle");
                int precioIndex = reader.GetOrdinal("precio_unitario");

                while (reader.Read())
                {
                    detalles.Add(new DetallePedido
                    {
                        IdProducto = reader.GetInt32(idProdIndex),
                        NombreProducto = reader.GetString(nombreProdIndex),
                        Cantidad = reader.GetInt32(cantidadIndex),
                        PrecioUnitario = reader.GetInt32(precioIndex)
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][PedidoDAO.ObtenerDetallesPorPedido] {ex.Message}");
            }

            return detalles;
        }


    }
}
