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
                int stockActual = productoSeleccionado.UbicacionesConStock?.Sum(u => u.Cantidad) ?? 0;
                if (cantidad > stockActual)
                {
                    MostrarMensaje($"Stock insuficiente. Disponible: {stockActual}", false);
                    return;
                }

                var detalle = new DetalleVenta
                {
                    IdProducto = productoSeleccionado.IdProducto,
                    ProductoNombre = productoSeleccionado.NombreProducto,
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
                MostrarMensaje("Seleccione un producto y cantidad válida.", false);
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

            int totalVenta = 0;
            foreach (var d in carrito)
                totalVenta += d.SubtotalDetalle;

            int idCliente;

            if (cmbCliente.SelectedItem is Cliente clienteSeleccionado)
            {
                idCliente = clienteSeleccionado.IdCliente;
            }
            else
            {
                idCliente = ObtenerClienteVentaExpress(); // Ya implementado más arriba
            }

            var venta = new Venta
            {
                IdCliente = idCliente,
                FechaVenta = DateTime.Now,
                TotalVenta = totalVenta
            };

            int idVenta = VentaDAO.RegistrarVenta(venta);

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
                    int stock = InventarioProductoDAO.ObtenerUbicacionesPorProducto(detalle.IdProducto)
                                                     .Sum(u => u.Cantidad);

                    if (nuevaCantidad > stock)
                    {
                        MostrarMensaje($"Stock insuficiente. Disponible: {stock}", false);
                        txt.Text = detalle.CantidadDetalle.ToString();
                        return;
                    }

                    detalle.CantidadDetalle = nuevaCantidad;
                    detalle.SubtotalDetalle = nuevaCantidad * detalle.PrecioUnitario;
                    RefrescarCarrito();
                }
                else
                {
                    MostrarMensaje("Cantidad inválida. Debe ser mayor a 0.", false);
                    txt.Text = detalle.CantidadDetalle.ToString();
                }
            }
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
                InicializarClientes(); // Recarga los clientes incluyendo el nuevo
                cmbCliente.SelectedItem = dialog.ClienteAgregado; // Selecciona automáticamente el nuevo
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
