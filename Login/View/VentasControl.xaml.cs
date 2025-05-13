using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Login.Model;
using Login.Data;

namespace Login.View
{
    public partial class VentasControl : UserControl
    {
        private List<Producto> productos = new();

        public VentasControl()
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
            if (cmbProducto.SelectedItem is Producto productoSeleccionado)
            {
                if (int.TryParse(txtCantidad.Text, out int cantidad))
                {
                    if (cantidad <= 0)
                    {
                        MostrarMensaje("Cantidad inválida.", false);
                        return;
                    }

                    // Obtener stock actual desde la base de datos
                    int stockActual = ProductoDAO.ObtenerStockPorId(productoSeleccionado.IdProducto);

                    if (cantidad > stockActual)
                    {
                        MostrarMensaje($"Stock insuficiente. Disponible: {stockActual}", false);
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

                    // Actualizar el stock real
                    ProductoDAO.ActualizarStock(productoSeleccionado.IdProducto, stockActual - cantidad);

                    lstVentas.Items.Add($"Vendiste {cantidad} x {productoSeleccionado.NombreProducto} - Total: ${subtotal}");
                    MostrarMensaje("Venta registrada correctamente.", true);

                    // Refrescar vista
                    InicializarProductos();
                    ActualizarListas();
                    cmbProducto.SelectedItem = null;
                    txtCantidad.Text = string.Empty;
                }
                else
                {
                    MostrarMensaje("Ingrese una cantidad válida.", false);
                }
            }
            else
            {
                MostrarMensaje("Seleccione un producto.", false);
            }
        }


        private void MostrarMensaje(string texto, bool esExito)
        {
            lblMensaje.Text = texto;
            lblMensaje.Foreground = esExito ? System.Windows.Media.Brushes.LightGreen : System.Windows.Media.Brushes.Salmon;
        }
    }
}
