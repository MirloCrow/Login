using FERCO.Model;
using FERCO.ViewModel;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace FERCO.Data
{
    public static class ReparacionDAO
    {
        public static bool CrearReparacionConDetalle(int idCliente, List<TipoReparacionProductoEditable> productos, out string mensajeError)
        {
            mensajeError = "";

            using var conn = DAOHelper.AbrirConexionSegura();
            using var trans = conn.BeginTransaction();
            try
            {
                // 1. Insertar reparación base
                SqlCommand cmdInsertReparacion = new(
                    "INSERT INTO Reparacion (id_cliente, fecha_reparacion, costo_reparacion, estado) " +
                    "OUTPUT INSERTED.id_reparacion " +
                    "VALUES (@cliente, GETDATE(), @total, 'Pendiente')", conn, trans);

                int totalReparacion = 0;
                foreach (var p in productos)
                    totalReparacion += p.CantidadAUsar * ObtenerPrecioProducto(conn, trans, p.IdProducto);

                cmdInsertReparacion.Parameters.AddWithValue("@cliente", idCliente);
                cmdInsertReparacion.Parameters.AddWithValue("@total", totalReparacion);

                int idReparacion = (int)cmdInsertReparacion.ExecuteScalar();

                // 2. Insertar detalle y descontar stock
                foreach (var item in productos)
                {
                    int stockRestante = item.CantidadAUsar;
                    int precio = ObtenerPrecioProducto(conn, trans, item.IdProducto);

                    // Obtener inventarios con stock (mayor stock primero)
                    var inventarios = InventarioProductoDAO.ObtenerInventariosConStock(conn, trans, item.IdProducto);

                    foreach (var inv in inventarios)
                    {
                        if (stockRestante <= 0) break;

                        int aDescontar = Math.Min(inv.Cantidad, stockRestante);

                        // Actualizar stock
                        SqlCommand cmdUpdateStock = new(
                            "UPDATE InventarioProducto " +
                            "SET cantidad = cantidad - @cant, fecha_actualizacion = GETDATE() " +
                            "WHERE id_inventario = @inv AND id_producto = @prod", conn, trans);
                        cmdUpdateStock.Parameters.AddWithValue("@cant", aDescontar);
                        cmdUpdateStock.Parameters.AddWithValue("@inv", inv.IdInventario);
                        cmdUpdateStock.Parameters.AddWithValue("@prod", item.IdProducto);
                        cmdUpdateStock.ExecuteNonQuery();

                        // Insertar detalle
                        SqlCommand cmdInsertDetalle = new(
                            "INSERT INTO Detalle_Reparacion (id_reparacion, id_producto, cantidad, costo_unitario, subtotal_detalle) " +
                            "VALUES (@rep, @prod, @cant, @precio, @sub)", conn, trans);
                        cmdInsertDetalle.Parameters.AddWithValue("@rep", idReparacion);
                        cmdInsertDetalle.Parameters.AddWithValue("@prod", item.IdProducto);
                        cmdInsertDetalle.Parameters.AddWithValue("@cant", aDescontar);
                        cmdInsertDetalle.Parameters.AddWithValue("@precio", precio);
                        cmdInsertDetalle.Parameters.AddWithValue("@sub", precio * aDescontar);
                        cmdInsertDetalle.ExecuteNonQuery();

                        stockRestante -= aDescontar;
                    }

                    if (stockRestante > 0)
                    {
                        trans.Rollback();
                        mensajeError = $"No hay suficiente stock del producto '{item.NombreProducto}' (faltan {stockRestante} unidades).";
                        return false;
                    }
                }

                trans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                mensajeError = "Error en la reparación: " + ex.Message;
                return false;
            }
        }

        private static int ObtenerPrecioProducto(SqlConnection conn, SqlTransaction trans, int idProducto)
        {
            SqlCommand cmd = new("SELECT precio_producto FROM Producto WHERE id_producto = @id", conn, trans);
            cmd.Parameters.AddWithValue("@id", idProducto);
            return (int)cmd.ExecuteScalar();
        }
    }
}
