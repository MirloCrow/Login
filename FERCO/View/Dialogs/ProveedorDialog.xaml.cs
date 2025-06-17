using System.Windows;
using FERCO.Data;
using FERCO.Model;

namespace FERCO.View.Dialogs
{
    public partial class ProveedorDialog : Window
    {
        public Proveedor ProveedorEditado { get; private set; }

        public ProveedorDialog(Proveedor? proveedor = null)
        {
            InitializeComponent();

            if (proveedor != null)
            {
                // Edición
                txtNombre.Text = proveedor.Nombre;
                txtEmail.Text = proveedor.Email;
                txtDireccion.Text = proveedor.Direccion;
                txtTelefono.Text = proveedor.Telefono.ToString();

                ProveedorEditado = proveedor;
                Title = "Editar Proveedor";
            }
            else
            {
                // Nuevo
                ProveedorEditado = new Proveedor();
                Title = "Nuevo Proveedor";
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            ProveedorEditado.Nombre = txtNombre.Text.Trim();
            ProveedorEditado.Email = txtEmail.Text.Trim();
            ProveedorEditado.Direccion = txtDireccion.Text.Trim();

            if (!int.TryParse(txtTelefono.Text, out int telefono))
            {
                MessageBox.Show("Teléfono inválido.");
                return;
            }
            ProveedorEditado.Telefono = telefono;

            if (string.IsNullOrWhiteSpace(ProveedorEditado.Nombre))
            {
                MessageBox.Show("Debe ingresar un nombre.");
                return;
            }

            bool resultado;
            if (ProveedorEditado.IdProveedor == 0)
            {
                resultado = ProveedorDAO.Agregar(ProveedorEditado);
                ProveedorEditado = ProveedorDAO.ObtenerUltimo()!;
            }
            else
            {
                resultado = ProveedorDAO.Actualizar(ProveedorEditado);
            }

            if (resultado)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("No se pudo guardar el proveedor.");
            }
        }
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

    }
}
