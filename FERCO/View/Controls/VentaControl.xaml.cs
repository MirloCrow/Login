using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using FERCO.Model;
using FERCO.Data;

namespace FERCO.View
{
    public partial class VentaControl : UserControl
    {
        private List<Producto> productos = [];

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

        private void BtnRegistrarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducto.SelectedItem is not Producto productoSeleccionado)
            {
                MostrarMensaje("Seleccione un producto.", false);
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MostrarMensaje("Cantidad inválida.", false);
                return;
            }

            if (cmbUbicacion.SelectedItem is not InventarioProductoView ubicacionSeleccionada)
            {
                MostrarMensaje("Debe seleccionar una ubicación de inventario.", false);
                return;
            }

            if (cantidad > ubicacionSeleccionada.Cantidad)
            {
                MostrarMensaje($"Stock insuficiente en la ubicación '{ubicacionSeleccionada.DescripcionUbicacion}'. Disponible: {ubicacionSeleccionada.Cantidad}", false);
                return;
            }

            int subtotal = cantidad * productoSeleccionado.PrecioProducto;

            var venta = new Venta
            {
                IdCliente = 1, // cliente genérico
                FechaVenta = DateTime.Now,
                TotalVenta = subtotal
            };

            int idVenta = VentaDAO.RegistrarVenta(venta);

            var detalle = new DetalleVenta
            {
                IdVenta = idVenta,
                IdProducto = productoSeleccionado.IdProducto,
                PrecioUnitario = productoSeleccionado.PrecioProducto,
                CantidadDetalle = cantidad,
                SubtotalDetalle = subtotal
            };

            VentaDAO.RegistrarDetalle(detalle);

            bool actualizado = InventarioProductoDAO.ActualizarStock(
                productoSeleccionado.IdProducto,
                ubicacionSeleccionada.IdInventario,
                ubicacionSeleccionada.Cantidad - cantidad);

            if (!actualizado)
            {
                MostrarMensaje("Error al actualizar el stock.", false);
                return;
            }

            lstVentas.Items.Add($"Vendiste {cantidad} x {productoSeleccionado.NombreProducto} - Total: ${subtotal}");
            MostrarMensaje("Venta registrada correctamente.", true);

            // Refrescar vista
            InicializarProductos();
            ActualizarListas();
            cmbProducto.SelectedItem = null;
            cmbUbicacion.SelectedItem = null;
            txtCantidad.Text = string.Empty;
        }


        private void cmbProducto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProducto.SelectedItem is Producto producto)
            {
                CargarUbicacionesParaProducto(producto.IdProducto);
            }
        }

        private void CargarUbicacionesParaProducto(int idProducto)
        {
            var ubicaciones = InventarioProductoDAO.ObtenerProductosPorUbicacionDeProducto(idProducto);
            cmbUbicacion.ItemsSource = ubicaciones;
            cmbUbicacion.DisplayMemberPath = nameof(InventarioProductoView.DescripcionUbicacion);
            cmbUbicacion.SelectedValuePath = nameof(InventarioProductoView.IdInventario);
        }


        private void MostrarMensaje(string texto, bool esExito)
        {
            lblMensaje.Text = texto;
            lblMensaje.Foreground = esExito ? System.Windows.Media.Brushes.LightGreen : System.Windows.Media.Brushes.Salmon;
        }
    }
}
