using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FERCO.Model;
using FERCO.Data;
using FERCO.Utilities;
using FERCO.View.Dialogs;

namespace FERCO.View
{
    public partial class VentaControl : UserControl
    {
        private List<Producto> productos = [];
        private readonly List<DetalleVenta> carrito = [];

        public VentaControl()
        {
            InitializeComponent();
            InicializarClientes();
            InicializarProductos();
            ActualizarListas();
        }

        private void InicializarProductos()
        {
            productos = ProductoDAO.ObtenerTodos();
            cmbProducto.ItemsSource = productos;
            cmbProducto.DisplayMemberPath = "NombreProducto";
        }

        private void ActualizarListas()
        {
            productos = ProductoDAO.ObtenerTodos();
            lstStock.Items.Clear();
            foreach (var p in productos)
            {
                lstStock.Items.Add($"{p.NombreProducto} | Precio: ${p.PrecioProducto} | Stock total: {p.StockTotal}");
            }
        }

        private void BtnAgregarCarrito_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducto.SelectedItem is Producto productoSeleccionado &&
                int.TryParse(txtCantidad.Text, out int cantidad) &&
                cantidad > 0)
            {
                if (productoSeleccionado.UbicacionesConStock == null)
                {
                    MostrarMensaje("Error: no se pudo obtener el stock del producto.", false);
                    return;
                }

                // Validación centralizada
                if (!ValidarStockDisponible(productoSeleccionado.IdProducto, cantidad, null, out string mensaje))
                {
                    MostrarMensaje(mensaje, false);
                    return;
                }

                var detalle = new DetalleVenta
                {
                    IdProducto = productoSeleccionado.IdProducto,
                    NombreProducto = productoSeleccionado.NombreProducto,
                    PrecioUnitario = productoSeleccionado.PrecioProducto,
                    CantidadDetalle = cantidad,
                    SubtotalDetalle = cantidad * productoSeleccionado.PrecioProducto
                };

                carrito.Add(detalle);
                RefrescarCarrito();

                txtCantidad.Clear();
                cmbProducto.SelectedItem = null;
                MostrarMensaje("Producto agregado al carrito.", true);
            }
            else
            {
                MostrarMensaje("Seleccione un producto y una cantidad válida.", false);
            }
        }


        private void RefrescarCarrito()
        {
            lstCarrito.ItemsSource = null;
            lstCarrito.ItemsSource = carrito;
            ActualizarTotal();
        }

        private void ActualizarTotal()
        {
            int total = 0;
            foreach (var d in carrito)
                total += d.SubtotalDetalle;

            lblTotal.Text = $"Total: ${total}";
        }

        private void BtnEliminarItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is DetalleVenta detalle)
            {
                carrito.Remove(detalle);
                RefrescarCarrito();
                MostrarMensaje("Producto eliminado del carrito.", true);
            }
        }

        private void BtnRegistrarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (carrito.Count == 0)
            {
                MostrarMensaje("El carrito está vacío.", false);
                return;
            }

            // Validación de stock por cada producto
            foreach (var detalle in carrito)
            {
                if (!ValidarStockDisponible(detalle.IdProducto, detalle.CantidadDetalle, detalle, out string mensaje))
                {
                    MostrarMensaje($"No se puede registrar la venta: {mensaje}", false);
                    return;
                }
            }

            int totalVenta = carrito.Sum(d => d.SubtotalDetalle);

            Cliente? clienteSeleccionado;

            if (cmbCliente.SelectedItem is Cliente cliente)
            {
                clienteSeleccionado = cliente;
            }
            else
            {
                int idExpress = ObtenerClienteVentaExpress();
                clienteSeleccionado = ClienteDAO.ObtenerPorId(idExpress);

                if (clienteSeleccionado == null)
                {
                    MostrarMensaje("Error: no se pudo obtener el cliente Venta Express.", false);
                    return;
                }
            }

            var venta = new Venta
            {
                IdCliente = clienteSeleccionado.IdCliente,
                ClienteNombre = clienteSeleccionado.NombreCliente,
                FechaVenta = DateTime.Now,
                TotalVenta = totalVenta
            };


            int idVenta = VentaDAO.RegistrarVenta(venta);
            if (idVenta <= 0)
            {
                MostrarMensaje("No se pudo registrar la venta. Por favor, intente nuevamente.", false);
                return;
            }
            venta.IdVenta = idVenta;

            foreach (var detalle in carrito)
            {
                detalle.IdVenta = idVenta;
                VentaDAO.RegistrarDetalle(detalle);

                DescontarStock(detalle.IdProducto, detalle.CantidadDetalle);
            }

            lstVentas.Items.Add($"Venta registrada - Total: ${totalVenta}");
            MostrarMensaje("Venta completada exitosamente.", true);

            var copiaCarrito = new List<DetalleVenta>(carrito);

            carrito.Clear();
            RefrescarCarrito();
            lblTotal.Text = "Total: $0";

            InicializarProductos();
            ActualizarListas();

            BoletaHTMLGenerator.GenerarBoleta(venta, copiaCarrito);
        }

        private void MostrarMensaje(string texto, bool esExito)
        {
            lblMensaje.Text = texto;
            lblMensaje.Foreground = esExito ? System.Windows.Media.Brushes.LightGreen : System.Windows.Media.Brushes.Salmon;
        }

        private void Cantidad_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && txt.DataContext is DetalleVenta detalle)
            {
                if (int.TryParse(txt.Text, out int nuevaCantidad) && nuevaCantidad > 0)
                {
                    if (!ValidarStockDisponible(detalle.IdProducto, nuevaCantidad, detalle, out string mensaje))
                    {
                        MostrarMensaje(mensaje, false);
                        txt.Text = detalle.CantidadDetalle.ToString(); // Revertir
                        return;
                    }

                    detalle.CantidadDetalle = nuevaCantidad;
                    detalle.SubtotalDetalle = nuevaCantidad * detalle.PrecioUnitario;
                    RefrescarCarrito();
                }
                else
                {
                    MostrarMensaje("Cantidad inválida. Debe ser mayor a 0.", false);
                    txt.Text = detalle.CantidadDetalle.ToString(); // Revertir
                }
            }
        }

        private bool ValidarStockDisponible(int idProducto, int cantidadDeseada, DetalleVenta? lineaActual, out string mensaje)
        {
            var ubicaciones = InventarioProductoDAO.ObtenerUbicacionesPorProducto(idProducto);
            int stockTotal = ubicaciones.Sum(u => u.Cantidad);

            int cantidadEnOtrasLineas = carrito
                .Where(d => d.IdProducto == idProducto && d != lineaActual)
                .Sum(d => d.CantidadDetalle);

            int cantidadTotalDeseada = cantidadDeseada + cantidadEnOtrasLineas;

            if (cantidadTotalDeseada > stockTotal)
            {
                int disponible = stockTotal - cantidadEnOtrasLineas;

                if (stockTotal == 0)
                {
                    mensaje = "No hay stock disponible para este producto.";
                }
                else if (disponible <= 0)
                {
                    mensaje = $"Ya has asignado el máximo ({cantidadEnOtrasLineas}) en otras líneas.";
                }
                else
                {
                    mensaje = $"Stock insuficiente. Ya hay {cantidadEnOtrasLineas} en otras líneas. Máximo permitido aquí: {disponible}.";
                }

                return false;
            }

            mensaje = string.Empty;
            return true;
        }


        private static void DescontarStock(int idProducto, int cantidad)
        {
            var ubicaciones = InventarioProductoDAO.ObtenerUbicacionesPorProducto(idProducto);
            int cantidadARestar = cantidad;

            foreach (var u in ubicaciones)
            {
                if (cantidadARestar <= 0) break;

                int descontar = Math.Min(cantidadARestar, u.Cantidad);
                u.Cantidad -= descontar;
                cantidadARestar -= descontar;

                InventarioProductoDAO.Actualizar(u);
            }
        }
        private void InicializarClientes()
        {
            var clientes = ClienteDAO.ObtenerTodos()
                                     .OrderBy(c => c.NombreCliente)
                                     .ToList();

            cmbCliente.ItemsSource = clientes;
            cmbCliente.DisplayMemberPath = "NombreCliente";
        }

        private void BtnAgregarCliente_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ClienteDialog();
            if (dialog.ShowDialog() == true)
            {
                InicializarClientes(); // Recarga la lista

                // Buscar por ID para volver a seleccionar al cliente recién agregado
                var clienteAgregado = dialog.ClienteAgregado;

                if (clienteAgregado != null)
                {
                    var clienteSeleccionado = cmbCliente.Items.Cast<Cliente>()
                        .FirstOrDefault(c => c.IdCliente == clienteAgregado.IdCliente);

                    if (clienteSeleccionado != null)
                    {
                        cmbCliente.SelectedItem = clienteSeleccionado;
                    }
                }

            }
        }

        private static int ObtenerClienteVentaExpress()
        {
            const string nombreVentaExpress = "Venta Express";

            var cliente = ClienteDAO.BuscarPorNombre(nombreVentaExpress);
            if (cliente != null)
                return cliente.IdCliente;

            var nuevo = new Cliente
            {
                RutCliente = 99999999,
                NombreCliente = nombreVentaExpress,
                EstadoCliente = false,
                EmailCliente = "ventaexpress@ferco.local",
                DireccionCliente = "N/A",
                TelefonoCliente = 0
            };

            int nuevoId = ClienteDAO.AgregarYObtenerId(nuevo);
            return nuevoId;
        }
    }
}
