using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LoginApp
{
    public partial class VentanaVentas : Window
    {
        public class Producto
        {
            public string Nombre { get; set; }
            public double Precio { get; set; }
            public int Stock { get; set; }

            public override string ToString() => $"{Nombre} - Stock: {Stock}";
        }

        private List<Producto> productos = new();

        public VentanaVentas()
        {
            InitializeComponent();
            InicializarProductos();
            ActualizarListas();
        }

        private void InicializarProductos()
        {
            productos.Add(new Producto { Nombre = "Cámara de aire", Precio = 3000, Stock = 20 });
            productos.Add(new Producto { Nombre = "Pastillas de freno", Precio = 5000, Stock = 15 });
            productos.Add(new Producto { Nombre = "Lubricante", Precio = 2500, Stock = 10 });
            productos.Add(new Producto { Nombre = "Cadena 6v", Precio = 8000, Stock = 8 });

            cmbProducto.ItemsSource = productos;
            cmbProducto.DisplayMemberPath = "Nombre";
        }

        private void ActualizarListas()
        {
            lstStock.Items.Clear();
            foreach (var p in productos)
                lstStock.Items.Add(p.ToString());
        }

        private void BtnRegistrarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducto.SelectedItem is Producto productoSeleccionado)
            {
                if (int.TryParse(txtCantidad.Text, out int cantidad))
                {
                    if (cantidad > 0 && cantidad <= productoSeleccionado.Stock)
                    {
                        productoSeleccionado.Stock -= cantidad;
                        lstVentas.Items.Add($"Vendiste {cantidad} x {productoSeleccionado.Nombre} - Total: ${productoSeleccionado.Precio * cantidad}");
                        lblMensaje.Text = "Venta registrada correctamente.";
                        lblMensaje.Foreground = System.Windows.Media.Brushes.LightGreen;
                        ActualizarListas();
                    }
                    else
                    {
                        lblMensaje.Text = "Cantidad inválida o mayor al stock disponible.";
                        lblMensaje.Foreground = System.Windows.Media.Brushes.Salmon;
                    }
                }
                else
                {
                    lblMensaje.Text = "Ingrese un número válido.";
                    lblMensaje.Foreground = System.Windows.Media.Brushes.Salmon;
                }
            }
            else
            {
                lblMensaje.Text = "Seleccione un producto.";
                lblMensaje.Foreground = System.Windows.Media.Brushes.Salmon;
            }
        }
    }
}
