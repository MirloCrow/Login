using System.Collections.ObjectModel;
using System.Windows;
using FERCO.Data;
using FERCO.Model;

namespace FERCO.View.Dialogs
{
    public partial class TipoReparacionDialog : Window
    {
        private List<Producto> productosDisponibles = [];
        private ObservableCollection<TipoReparacionProducto> productosSeleccionados = [];

        public TipoReparacionDialog()
        {
            InitializeComponent();
            productosDisponibles = ProductoDAO.ObtenerTodos();
            cmbProducto.ItemsSource = productosDisponibles;
            dgProductos.ItemsSource = productosSeleccionados;
        }

        private void AgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducto.SelectedItem is Producto prod && int.TryParse(txtCantidad.Text, out int cantidad) && cantidad > 0)
            {
                productosSeleccionados.Add(new TipoReparacionProducto
                {
                    IdProducto = prod.IdProducto,
                    NombreProducto = prod.NombreProducto,
                    CantidadRequerida = cantidad
                });
                txtCantidad.Text = "";
                cmbProducto.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("Debes seleccionar un producto válido y cantidad > 0.");
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || productosSeleccionados.Count == 0)
            {
                MessageBox.Show("Debes ingresar un nombre y al menos un producto.");
                return;
            }

            var tipo = new TipoReparacion
            {
                Nombre = txtNombre.Text.Trim(),
                Descripcion = txtDescripcion.Text.Trim()
            };

            int idTipo = TipoReparacionDAO.Insertar(tipo);

            foreach (var p in productosSeleccionados)
            {
                TipoReparacionDAO.AgregarProductoATipo(idTipo, p.IdProducto, p.CantidadRequerida);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
