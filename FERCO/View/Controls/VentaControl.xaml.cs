using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FERCO.Model;
using FERCO.Data;
using FERCO.Utilidades;

namespace FERCO.View
{
    public partial class VentaControl : UserControl
    {
        private List<Producto> productos = [];
        private List<DetalleVenta> carrito = new();

        public VentaControl()
        {
            InitializeComponent();
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
                lstStock.Items.Add(p.ToString());
            }
        }

        private void BtnAgregarCarrito_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducto.SelectedItem is Producto productoSeleccionado &&
                int.TryParse(txtCantidad.Text, out int cantidad) &&
                cantidad > 0)
            {
                int stockActual = ProductoDAO.ObtenerStockPorId(productoSeleccionado.IdProducto);
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

            var venta = new Venta
            {
                IdCliente = 1,
                FechaVenta = DateTime.Now,
                TotalVenta = totalVenta
            };

            int idVenta = VentaDAO.RegistrarVenta(venta);

            foreach (var detalle in carrito)
            {
                detalle.IdVenta = idVenta;
                VentaDAO.RegistrarDetalle(detalle);

                int stockActual = ProductoDAO.ObtenerStockPorId(detalle.IdProducto);
                ProductoDAO.ActualizarStock(detalle.IdProducto, stockActual - detalle.CantidadDetalle);
            }

            lstVentas.Items.Add($"Venta registrada - Total: ${totalVenta}");
            MostrarMensaje("Venta completada exitosamente.", true);

            // ✅ Copia segura del carrito antes de limpiarlo
            var copiaCarrito = new List<DetalleVenta>(carrito);

            carrito.Clear();
            RefrescarCarrito();
            lblTotal.Text = "Total: $0";

            InicializarProductos();
            ActualizarListas();

            // ✅ Generar boleta con los datos correctos
            BoletaHTMLGenerator.GenerarBoleta(venta, copiaCarrito);
        }


        private void MostrarMensaje(string texto, bool esExito)
        {
            lblMensaje.Text = texto;
            lblMensaje.Foreground = esExito ? System.Windows.Media.Brushes.LightGreen : System.Windows.Media.Brushes.Salmon;
        }

        // ✅ MÉTODO NUEVO: Se ejecuta al editar cantidad en el carrito
        private void Cantidad_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox txt && txt.DataContext is DetalleVenta detalle)
            {
                if (int.TryParse(txt.Text, out int nuevaCantidad) && nuevaCantidad > 0)
                {
                    int stock = ProductoDAO.ObtenerStockPorId(detalle.IdProducto);

                    if (nuevaCantidad > stock)
                    {
                        MostrarMensaje($"Stock insuficiente. Disponible: {stock}", false);
                        txt.Text = detalle.CantidadDetalle.ToString(); // Revertir visualmente
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
    }
}
