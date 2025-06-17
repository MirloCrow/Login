using System.Windows;
using System.Windows.Controls;
using FERCO.Model;
using FERCO.Data;

namespace FERCO.View
{
    public partial class InventarioControl : UserControl
    {
        private Inventario? inventarioSeleccionado = null;

        public InventarioControl()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgInventario.ItemsSource = InventarioDAO.ObtenerInventarios();
            inventarioSeleccionado = null;
            txtDescripcion.Clear();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Ingrese una descripción para el inventario.");
                return;
            }

            var nuevo = new Inventario
            {
                Descripcion = txtDescripcion.Text.Trim()
            };

            InventarioDAO.Agregar(nuevo);
            CargarDatos();
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (inventarioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un inventario desde la tabla.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("La descripción no puede estar vacía.");
                return;
            }

            inventarioSeleccionado.Descripcion = txtDescripcion.Text.Trim();
            InventarioDAO.Actualizar(inventarioSeleccionado);
            CargarDatos();
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (inventarioSeleccionado == null)
            {
                MessageBox.Show("Seleccione un inventario para eliminar.");
                return;
            }

            InventarioDAO.Eliminar(inventarioSeleccionado.IdInventario);
            CargarDatos();
        }

        private void DgInventario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            inventarioSeleccionado = dgInventario.SelectedItem as Inventario;

            if (inventarioSeleccionado != null)
            {
                txtDescripcion.Text = inventarioSeleccionado.Descripcion;
            }
        }
    }
}
