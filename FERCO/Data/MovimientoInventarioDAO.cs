using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using FERCO.Model;

namespace FERCO.Data
{
    public static class MovimientoInventarioDAO
    {
        private static bool RegistrarMovimiento(int idProducto, int idInventario, int cantidad, string tipo, string motivo, int? idReferencia = null)
        {
            if (idProducto <= 0 || idInventario <= 0 || cantidad == 0 || string.IsNullOrWhiteSpace(tipo)) return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                const string query = @"
                    INSERT INTO MovimientoInventario (
                        id_producto,
                        id_inventario,
                        fecha_movimiento,
                        tipo_movimiento,
                        cantidad,
                        motivo,
                        id_referencia)
                    VALUES (
                        @idProducto,
                        @idInventario,
                        GETDATE(),
                        @tipo,
                        @cantidad,
                        @motivo,
                        @idReferencia)";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);
                cmd.Parameters.AddWithValue("@idInventario", idInventario);
                cmd.Parameters.AddWithValue("@tipo", tipo);
                cmd.Parameters.AddWithValue("@cantidad", cantidad);
                cmd.Parameters.AddWithValue("@motivo", motivo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@idReferencia", idReferencia ?? (object)DBNull.Value);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][MovimientoInventarioDAO.RegistrarMovimiento] {ex.Message}");
                return false;
            }
        }

        public static bool RegistrarEntrada(int idProducto, int idInventario, int cantidad, decimal costoUnitario, string motivo = "Compra", int? idReferencia = null)
        {
            bool exito = RegistrarMovimiento(idProducto, idInventario, cantidad, "ENTRADA", motivo, idReferencia);

            if (exito)
            {
                return ProductoDAO.ActualizarCostoPromedio(idProducto, cantidad, costoUnitario);
            }

            return false;
        }

        public static bool RegistrarSalida(int idProducto, int idInventario, int cantidad, string motivo = "Venta", int? idReferencia = null)
        {
            return RegistrarMovimiento(idProducto, idInventario, -Math.Abs(cantidad), "SALIDA", motivo, idReferencia);
        }

        public static bool RegistrarAjuste(int idProducto, int idInventario, int cantidad, string motivo = "Ajuste manual", int? idReferencia = null)
        {
            return RegistrarMovimiento(idProducto, idInventario, cantidad, "AJUSTE", motivo, idReferencia);
        }

        public static List<MovimientoInventario> ObtenerMovimientosPorProducto(int idProducto)
        {
            var movimientos = new List<MovimientoInventario>();

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                const string query = @"
                    SELECT * FROM MovimientoInventario
                    WHERE id_producto = @idProducto
                    ORDER BY fecha_movimiento DESC";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@idProducto", idProducto);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    movimientos.Add(new MovimientoInventario
                    {
                        IdMovimiento = (int)reader["id_movimiento"],
                        IdProducto = (int)reader["id_producto"],
                        IdInventario = (int)reader["id_inventario"],
                        FechaMovimiento = (DateTime)reader["fecha_movimiento"],
                        TipoMovimiento = reader["tipo_movimiento"]?.ToString() ?? "",
                        Cantidad = (int)reader["cantidad"],
                        Motivo = reader["motivo"]?.ToString(),
                        IdReferencia = reader["id_referencia"] == DBNull.Value ? null : (int?)reader["id_referencia"]
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][MovimientoInventarioDAO.ObtenerMovimientosPorProducto] {ex.Message}");
            }

            return movimientos;
        }
    }
}
