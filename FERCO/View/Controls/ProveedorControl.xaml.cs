using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FERCO.Data;
using FERCO.Model;

namespace FERCO.View
{
    /// <summary>
    /// Interaction logic for ProveedorControl.xaml
    /// </summary>
    public partial class ProveedorControl : UserControl
    {
        private Proveedor? proveedorSeleccionado;

        private void dgProveedores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            proveedorSeleccionado = dgProveedores.SelectedItem as Proveedor;
            if (proveedorSeleccionado != null)
            {
                txtNombre.Text = proveedorSeleccionado.Nombre;
                txtEmail.Text = proveedorSeleccionado.Email;
                txtDireccion.Text = proveedorSeleccionado.Direccion;
                txtTelefono.Text = proveedorSeleccionado.Telefono.ToString();
            }
        }
        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtTelefono.Text, out int telefono))
            {
                Proveedor proveedor = new Proveedor
                {
                    Nombre = txtNombre.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Direccion = txtDireccion.Text.Trim(),
                    Telefono = telefono
                };

                if (ProveedorDAO.Agregar(proveedor))
                {
                    MessageBox.Show("Proveedor agregado correctamente.");
                    CargarProveedores();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al agregar proveedor.");
                }
            }
            else
            {
                MessageBox.Show("Teléfono inválido.");
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (proveedorSeleccionado == null)
            {
                MessageBox.Show("Seleccione un proveedor para editar.");
                return;
            }

            if (int.TryParse(txtTelefono.Text, out int telefono))
            {
                proveedorSeleccionado.Nombre = txtNombre.Text.Trim();
                proveedorSeleccionado.Email = txtEmail.Text.Trim();
                proveedorSeleccionado.Direccion = txtDireccion.Text.Trim();
                proveedorSeleccionado.Telefono = telefono;

                if (ProveedorDAO.Actualizar(proveedorSeleccionado))
                {
                    MessageBox.Show("Proveedor actualizado.");
                    CargarProveedores();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al actualizar.");
                }
            }
            else
            {
                MessageBox.Show("Teléfono inválido.");
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (proveedorSeleccionado == null)
            {
                MessageBox.Show("Seleccione un proveedor para eliminar.");
                return;
            }

            var result = MessageBox.Show("¿Está seguro que desea eliminar este proveedor?", "Confirmar", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                if (ProveedorDAO.Eliminar(proveedorSeleccionado.IdProveedor))
                {
                    MessageBox.Show("Proveedor eliminado.");
                    CargarProveedores();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar.");
                }
            }
        }
        private void CargarProveedores()
        {
            dgProveedores.ItemsSource = null;
            dgProveedores.ItemsSource = ProveedorDAO.ObtenerTodos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtEmail.Text = "";
            txtDireccion.Text = "";
            txtTelefono.Text = "";
            proveedorSeleccionado = null;
        }


        public ProveedorControl()
        {
            InitializeComponent();
        }

    }
}
