using FERCO.Model;
using FERCO.ViewModel;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace FERCO.Data
{
    public static class ReparacionDAO
    {
        public static bool CrearReparacionConDetalle(int idCliente, int idTipoReparacion, List<TipoReparacionProductoEditable> productos, out string mensajeError)
        {
            mensajeError = "";

            using var conn = DAOHelper.AbrirConexionSegura();
            using var trans = conn.BeginTransaction();

            // Validar stock suficiente
            foreach (var item in productos)
            {
                int stockTotal = InventarioProductoDAO.ObtenerStockTotal(conn, trans, item.IdProducto);
                if (stockTotal < item.CantidadAUsar)
                {
                    mensajeError = $"Stock insuficiente para '{item.NombreProducto}': requiere {item.CantidadAUsar}, disponible {stockTotal}.";
                    return false;
                }
            }

            try
            {
                // Calcular costo total
                int totalReparacion = productos.Sum(p => p.CantidadAUsar * ObtenerPrecioProducto(conn, trans, p.IdProducto));

                // Insertar reparación con id_tipo_reparacion incluido
                SqlCommand cmdInsertReparacion = new(
                    "INSERT INTO Reparacion (id_cliente, id_tipo_reparacion, fecha_reparacion, costo_reparacion, estado) " +
                    "OUTPUT INSERTED.id_reparacion " +
                    "VALUES (@cliente, @tipo, GETDATE(), @total, 'Pendiente')", conn, trans);

                cmdInsertReparacion.Parameters.AddWithValue("@cliente", idCliente);
                cmdInsertReparacion.Parameters.AddWithValue("@tipo", idTipoReparacion); // ✅ Nuevo parámetro
                cmdInsertReparacion.Parameters.AddWithValue("@total", totalReparacion);

                int idReparacion = (int)cmdInsertReparacion.ExecuteScalar();

                // ✅ Obtener nombre del tipo para movimiento
                string motivoMovimiento = $"Uso en reparación: {TipoReparacionDAO.ObtenerNombrePorId(conn, trans, idTipoReparacion)}";

                // Insertar detalles y descontar stock
                foreach (var item in productos)
                {
                    int stockRestante = item.CantidadAUsar;
                    int precio = ObtenerPrecioProducto(conn, trans, item.IdProducto);

                    var inventarios = InventarioProductoDAO.ObtenerInventariosConStock(conn, trans, item.IdProducto);

                    foreach (var inv in inventarios)
                    {
                        if (stockRestante <= 0) break;

                        int aDescontar = Math.Min(inv.Cantidad, stockRestante);

                        // Actualizar stock
                        SqlCommand cmdUpdateStock = new(
                            "UPDATE InventarioProducto SET cantidad = cantidad - @cant, fecha_actualizacion = GETDATE() " +
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

                        // Registrar salida
                        MovimientoInventarioDAO.RegistrarSalida(
                            item.IdProducto,
                            inv.IdInventario,
                            aDescontar,
                            motivoMovimiento,
                            idReparacion
                        );

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
        public static List<ReparacionEditable> ObtenerTodas()
        {
            List<ReparacionEditable> lista = [];

            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new(
                "SELECT r.id_reparacion, r.fecha_reparacion, r.costo_reparacion, r.estado, c.nombre_cliente " +
                "FROM Reparacion r JOIN Cliente c ON r.id_cliente = c.id_cliente", conn);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new ReparacionEditable
                {
                    IdReparacion = (int)reader["id_reparacion"],
                    Fecha = (DateTime)reader["fecha_reparacion"],
                    Costo = (int)reader["costo_reparacion"],
                    Estado = reader["estado"].ToString() ?? "",
                    NombreCliente = reader["nombre_cliente"].ToString() ?? ""
                });
            }

            return lista;
        }

        public static List<Reparacion> ObtenerPorCliente(int idCliente)
        {
            List<Reparacion> lista = [];

            using var conn = ConexionBD.ObtenerConexion();
            conn.Open();

            var cmd = new SqlCommand("SELECT * FROM Reparacion WHERE id_cliente = @id ORDER BY fecha_reparacion DESC", conn);
            cmd.Parameters.AddWithValue("@id", idCliente);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Reparacion
                {
                    IdReparacion = (int)reader["id_reparacion"],
                    IdCliente = (int)reader["id_cliente"],
                    FechaReparacion = (DateTime)reader["fecha_reparacion"],
                    CostoReparacion = (int)reader["costo_reparacion"],
                    Estado = reader["estado"].ToString()!
                });
            }

            return lista;
        }

        public static bool ActualizarEstado(int idReparacion, string nuevoEstado)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new(
                "UPDATE Reparacion SET estado = @estado WHERE id_reparacion = @id", conn);
            cmd.Parameters.AddWithValue("@estado", nuevoEstado);
            cmd.Parameters.AddWithValue("@id", idReparacion);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static List<DetalleReparacion> ObtenerDetalles(int idReparacion)
        {
            List<DetalleReparacion> lista = [];

            using var conn = DAOHelper.AbrirConexionSegura();
            var cmd = new SqlCommand(@"
        SELECT dr.*, p.nombre_producto
        FROM Detalle_Reparacion dr
        JOIN Producto p ON dr.id_producto = p.id_producto
        WHERE dr.id_reparacion = @id", conn);

            cmd.Parameters.AddWithValue("@id", idReparacion);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new DetalleReparacion
                {
                    IdDetalleReparacion = (int)reader["id_detalle"],
                    IdReparacion = (int)reader["id_reparacion"],
                    IdProducto = (int)reader["id_producto"],
                    Cantidad = (int)reader["cantidad"],
                    PrecioUnitario = (int)reader["costo_unitario"],
                    NombreProducto = reader["nombre_producto"].ToString() ?? ""
                });
            }

            return lista;
        }

        public static Reparacion? ObtenerPorId(int idReparacion)
        {
            using var conn = DAOHelper.AbrirConexionSegura();

            var cmd = new SqlCommand(@"
        SELECT r.*, tr.nombre AS nombre_tipo, c.nombre_cliente
        FROM Reparacion r
        LEFT JOIN TipoReparacion tr ON r.id_tipo_reparacion = tr.id_tipo_reparacion
        LEFT JOIN Cliente c ON r.id_cliente = c.id_cliente
        WHERE r.id_reparacion = @id", conn);

            cmd.Parameters.AddWithValue("@id", idReparacion);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Reparacion
                {
                    IdReparacion = (int)reader["id_reparacion"],
                    IdCliente = (int)reader["id_cliente"],
                    FechaReparacion = (DateTime)reader["fecha_reparacion"],
                    Estado = reader["estado"].ToString() ?? "",
                    CostoReparacion = (int)reader["costo_reparacion"],
                    NombreCliente = reader["nombre_cliente"].ToString() ?? "",
                    NombreTipoReparacion = reader["nombre_tipo"].ToString() ?? ""
                };
            }

            return null;
        }
    }
}
