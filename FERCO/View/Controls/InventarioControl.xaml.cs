using System.Windows;
using System.Windows.Controls;
using FERCO.Model;
using FERCO.Data;
using System.Collections.Generic;

namespace FERCO.View
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
            dgInventario.ItemsSource = InventarioDAO.ObtenerUbicacionesConProductos();
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdInventario.Text) || string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("Completa ID y descripción.");
                return;
            }

            if (!int.TryParse(txtIdInventario.Text, out int idInventario))
            {
                MessageBox.Show("ID Inventario debe ser numérico.");
                return;
            }

            var ubicacion = new UbicacionInventario
            {
                IdInventario = idInventario,
                Descripcion = txtDescripcion.Text.Trim()
            };

            if (InventarioDAO.AgregarUbicacion(ubicacion))
            {
                MessageBox.Show("Ubicación agregada.");
                LimpiarCampos();
                CargarDatos();
            }
            else
            {
                MessageBox.Show("No se pudo agregar la ubicación.");
            }
        }

        private void BtnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtIdInventario.Text, out int idInventario) || string.IsNullOrWhiteSpace(txtDescripcion.Text))
            {
                MessageBox.Show("ID y descripción válidos requeridos.");
                return;
            }

            var ubicacion = new UbicacionInventario
            {
                IdInventario = idInventario,
                Descripcion = txtDescripcion.Text.Trim()
            };

            if (InventarioDAO.ActualizarUbicacion(ubicacion))
            {
                MessageBox.Show("Ubicación actualizada.");
                LimpiarCampos();
                CargarDatos();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar.");
            }
        }

        private void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtIdInventario.Text, out int idInventario))
            {
                MessageBox.Show("ID válido requerido.");
                return;
            }

            var confirmar = MessageBox.Show("¿Eliminar esta ubicación?", "Confirmar", MessageBoxButton.YesNo);
            if (confirmar == MessageBoxResult.Yes)
            {
                if (InventarioDAO.EliminarUbicacion(idInventario))
                {
                    MessageBox.Show("Ubicación eliminada.");
                    LimpiarCampos();
                    CargarDatos();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar.");
                }
            }
        }

        private void dgInventario_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgInventario.SelectedItem is UbicacionInventario ubicacion)
            {
                txtIdInventario.Text = ubicacion.IdInventario.ToString();
                txtDescripcion.Text = ubicacion.Descripcion;

                // Mostrar productos en la ubicación seleccionada
                dgProductosEnUbicacion.ItemsSource = InventarioDAO.ObtenerProductosPorUbicacion(ubicacion.IdInventario);
            }
        }

        private void LimpiarCampos()
        {
            txtIdInventario.Text = "";
            txtDescripcion.Text = "";
            dgProductosEnUbicacion.ItemsSource = null;
        }
    }
}
