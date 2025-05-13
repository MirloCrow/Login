using System.Windows;
using System.Windows.Controls;
using Login.Model;
using Login.Data;

namespace Login.View
{
    public partial class InventarioControl : UserControl
    {
        public InventarioControl()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            dgInventario.ItemsSource = InventarioDAO.ObtenerInventario();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (CamposValidos(out int id, out int prod, out int cantidad))
            {
                var inv = new Inventario
                {
                    IdInventario = id,
                    IdProducto = prod,
                    CantidadProducto = cantidad
                };
                InventarioDAO.Agregar(inv);
                CargarDatos();
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (CamposValidos(out int id, out int prod, out int cantidad))
            {
                var inv = new Inventario
                {
                    IdInventario = id,
                    IdProducto = prod,
                    CantidadProducto = cantidad
                };
                InventarioDAO.Actualizar(inv);
                CargarDatos();
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtId.Text, out int id))
            {
                InventarioDAO.Eliminar(id);
                CargarDatos();
            }
            else
            {
                MessageBox.Show("Ingrese un ID válido para eliminar.");
            }
        }

        private bool CamposValidos(out int id, out int prod, out int cantidad)
        {
            id = prod = cantidad = 0;
            if (string.IsNullOrWhiteSpace(txtId.Text) ||
                string.IsNullOrWhiteSpace(txtProducto.Text) ||
                string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.");
                return false;
            }

            return int.TryParse(txtId.Text, out id) &&
                   int.TryParse(txtProducto.Text, out prod) &&
                   int.TryParse(txtCantidad.Text, out cantidad);
        }
    }
}
