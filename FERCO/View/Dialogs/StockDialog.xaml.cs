using FERCO.Data;
using FERCO.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace FERCO.View.Dialogs
{
    public partial class StockDialog : Window
    {
        private readonly int idProducto;
        public InventarioProducto? RegistroGenerado { get; private set; }

        public StockDialog(int idProducto)
        {
            InitializeComponent();
            this.idProducto = idProducto;
            CargarInventarios();
        }

        private void CargarInventarios()
        {
            cmbInventario.ItemsSource = InventarioDAO.ObtenerInventarios();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtCantidad.Text.Trim(), out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbInventario.SelectedItem is not Inventario inventario)
            {
                MessageBox.Show("Seleccione una ubicación de inventario.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var ubicaciones = InventarioProductoDAO.ObtenerUbicacionesPorProducto(idProducto);
            var existente = ubicaciones.FirstOrDefault(u => u.IdInventario == inventario.IdInventario);

            int nuevaCantidad = existente != null ? existente.Cantidad + cantidad : cantidad;

            RegistroGenerado = new InventarioProducto
            {
                IdProducto = idProducto,
                IdInventario = inventario.IdInventario,
                Cantidad = nuevaCantidad
            };

            bool ok = existente != null
                ? InventarioProductoDAO.Actualizar(RegistroGenerado)
                : InventarioProductoDAO.Insertar(RegistroGenerado);

            if (ok)
            {
                MessageBox.Show("Stock actualizado.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Error al actualizar el stock.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
