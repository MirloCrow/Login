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

                const string query = @"
                SELECT p.id_producto,
                p.codigo_producto,
                p.nombre_producto, 
                p.descripcion_producto, 
                p.precio_producto, 
                p.costo_promedio,
                p.costo_unitario,
                p.id_proveedor, 
                p.id_categoria,
                pr.nombre_proveedor,
                c.nombre_categoria
                FROM Producto p
                JOIN Proveedor pr ON p.id_proveedor = pr.id_proveedor
                JOIN Categoria c ON p.id_categoria = c.id_categoria";

                using var cmd = new SqlCommand(query, conn);
                using var reader = cmd.ExecuteReader();

                int idIndex = reader.GetOrdinal("id_producto");
                int codigoIndex = reader.GetOrdinal("codigo_producto");
                int nombreIndex = reader.GetOrdinal("nombre_producto");
                int descripcionIndex = reader.GetOrdinal("descripcion_producto");
                int precioIndex = reader.GetOrdinal("precio_producto");
                int idProvIndex = reader.GetOrdinal("id_proveedor");
                int idCatIndex = reader.GetOrdinal("id_categoria");
                int nomProvIndex = reader.GetOrdinal("nombre_proveedor");
                int nomCatIndex = reader.GetOrdinal("nombre_categoria");
                int costoPromIndex = reader.GetOrdinal("costo_promedio");
                int costoUnitIndex = reader.GetOrdinal("costo_unitario");

                while (reader.Read())
                {
                    int idProducto = reader.GetInt32(idIndex);

                    var producto = new Producto
                    {
                        IdProducto = idProducto,
                        CodigoProducto = reader.GetString(codigoIndex),
                        NombreProducto = reader.GetString(nombreIndex),
                        DescripcionProducto = reader.GetString(descripcionIndex),
                        PrecioProducto = reader.GetInt32(precioIndex),
                        IdProveedor = reader.GetInt32(idProvIndex),
                        IdCategoria = reader.GetInt32(idCatIndex),
                        NombreProveedor = reader.GetString(nomProvIndex),
                        NombreCategoria = reader.GetString(nomCatIndex),
                        CostoPromedio = reader.IsDBNull(costoPromIndex) ? 0m : reader.GetDecimal(costoPromIndex),
                        CostoUnitario = reader.IsDBNull(costoUnitIndex) ? 0m : reader.GetDecimal(costoUnitIndex),
                        UbicacionesConStock = InventarioProductoDAO.ObtenerUbicacionesPorProducto(idProducto)
                    };

                    productos.Add(producto);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.ObtenerTodos] {ex.Message}");
            }

            return productos;
        }
        public static bool AgregarConEntradaInventario(Producto producto, int idInventario, int cantidadInicial, decimal costoUnitario)
        {
            if (!Agregar(producto)) return false;

            int idProducto = ObtenerUltimoId();

            // Actualizar costo promedio antes de registrar la entrada
            ActualizarCostoPromedio(idProducto, cantidadInicial, costoUnitario);

            // Registrar entrada en inventario
            return MovimientoInventarioDAO.RegistrarEntrada(idProducto, idInventario, cantidadInicial, costoUnitario);
        }

        public static bool ActualizarCostoPromedio(int idProducto, int cantidadEntrante, decimal costoUnitarioNuevo)
        {
            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                const string querySelect = @"
            SELECT 
                ISNULL(SUM(cantidad), 0) AS StockActual,
                (SELECT costo_promedio FROM Producto WHERE id_producto = @idProducto) AS CostoPromedioActual
            FROM InventarioProducto
            WHERE id_producto = @idProducto";

                int stockActual = 0;
                decimal costoPromedioActual = 0;

                using (var cmdSelect = new SqlCommand(querySelect, conn))
                {
                    cmdSelect.Parameters.AddWithValue("@idProducto", idProducto);
                    using var reader = cmdSelect.ExecuteReader();
                    if (reader.Read())
                    {
                        stockActual = reader.GetInt32(0);
                        costoPromedioActual = reader.IsDBNull(1) ? 0 : reader.GetDecimal(1);
                    }
                }

                int nuevoStock = stockActual + cantidadEntrante;
                if (nuevoStock == 0) return true; // evitar división por 0

                decimal nuevoCostoPromedio = ((stockActual * costoPromedioActual) + (cantidadEntrante * costoUnitarioNuevo)) / nuevoStock;

                const string queryUpdate = @"
            UPDATE Producto 
            SET 
                costo_promedio = @nuevoCosto, 
                costo_unitario = @ultimoCosto 
            WHERE id_producto = @idProducto";

                using var cmdUpdate = new SqlCommand(queryUpdate, conn);
                cmdUpdate.Parameters.AddWithValue("@nuevoCosto", nuevoCostoPromedio);
                cmdUpdate.Parameters.AddWithValue("@ultimoCosto", costoUnitarioNuevo);
                cmdUpdate.Parameters.AddWithValue("@idProducto", idProducto);

                return DAOHelper.EjecutarNoQuery(cmdUpdate);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.ActualizarCostoPromedio] {ex.Message}");
                return false;
            }
        }

        public static Producto? BuscarPorNombre(string nombre)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new("SELECT * FROM Producto WHERE nombre_producto = @nombre", conn);
            cmd.Parameters.AddWithValue("@nombre", nombre);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Producto
                {
                    IdProducto = (int)reader["id_producto"],
                    NombreProducto = reader["nombre_producto"].ToString() ?? "",
                    DescripcionProducto = reader["descripcion_producto"].ToString() ?? "",
                    PrecioProducto = (int)reader["precio_producto"],
                    IdCategoria = (int)reader["id_categoria"],
                    IdProveedor = (int)reader["id_proveedor"]
                };
            }
            return null;
        }

        public static Producto? BuscarPorCodigo(string codigo)
        {
            using var conn = DAOHelper.AbrirConexionSegura();
            SqlCommand cmd = new("SELECT * FROM Producto WHERE codigo_producto = @codigo", conn);
            cmd.Parameters.AddWithValue("@codigo", codigo);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Producto
                {
                    IdProducto = (int)reader["id_producto"],
                    CodigoProducto = reader["codigo_producto"].ToString() ?? "",
                    NombreProducto = reader["nombre_producto"].ToString() ?? "",
                    DescripcionProducto = reader["descripcion_producto"].ToString() ?? "",
                    PrecioProducto = (int)reader["precio_producto"],
                    IdCategoria = (int)reader["id_categoria"],
                    IdProveedor = (int)reader["id_proveedor"]
                };
            }
            return null;
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

        // CRUD Producto
        public static bool Agregar(Producto producto)
        {
            if (string.IsNullOrWhiteSpace(producto.NombreProducto) ||
                string.IsNullOrWhiteSpace(producto.CodigoProducto) ||
                producto.PrecioProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                const string query = @"
            INSERT INTO Producto (
                id_proveedor, 
                id_categoria, 
                nombre_producto, 
                descripcion_producto, 
                precio_producto, 
                codigo_producto)
            VALUES (
                @proveedor, 
                @categoria, 
                @nombre, 
                @descripcion, 
                @precio, 
                @codigoproducto)";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@proveedor", producto.IdProveedor);
                cmd.Parameters.AddWithValue("@categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@nombre", producto.NombreProducto);
                cmd.Parameters.AddWithValue("@descripcion", producto.DescripcionProducto);
                cmd.Parameters.AddWithValue("@precio", producto.PrecioProducto);
                cmd.Parameters.AddWithValue("@codigoproducto", producto.CodigoProducto);

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
            if (producto.IdProducto <= 0 ||
                string.IsNullOrWhiteSpace(producto.NombreProducto) ||
                string.IsNullOrWhiteSpace(producto.CodigoProducto) ||
                producto.PrecioProducto < 0)
                return false;

            try
            {
                using var conn = DAOHelper.AbrirConexionSegura();

                const string query = @"
            UPDATE Producto
            SET 
                codigo_producto = @codigo,
                id_proveedor = @proveedor,
                id_categoria = @categoria,
                nombre_producto = @nombre,
                descripcion_producto = @descripcion,
                precio_producto = @precio
            WHERE id_producto = @id";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", producto.IdProducto);
                cmd.Parameters.AddWithValue("@proveedor", producto.IdProveedor);
                cmd.Parameters.AddWithValue("@categoria", producto.IdCategoria);
                cmd.Parameters.AddWithValue("@nombre", producto.NombreProducto);
                cmd.Parameters.AddWithValue("@descripcion", producto.DescripcionProducto);
                cmd.Parameters.AddWithValue("@precio", producto.PrecioProducto);
                cmd.Parameters.AddWithValue("@codigo", producto.CodigoProducto);

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

                // IMPORTANTE: Considerar si se deben eliminar también las referencias
                // en InventarioProducto, Detalle_Pedido, Detalle_Venta, etc.
                // Aquí solo se elimina el producto, por lo que debe haber control de FK en la BD

                const string query = "DELETE FROM Producto WHERE id_producto = @id";
                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", idProducto);

                return DAOHelper.EjecutarNoQuery(cmd);
            }
            catch (SqlException sqlEx)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.Eliminar][SQL] {sqlEx.Message}");

                // Ejemplo: FK conflict
                if (sqlEx.Number == 547) // Constraint violation (clave foránea)
                {
                    Console.Error.WriteLine("No se puede eliminar el producto porque está en uso en otras tablas.");
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR][ProductoDAO.Eliminar] {ex.Message}");
                return false;
            }
        }
    }
}
