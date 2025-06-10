using System.Windows;
using FERCO.Data;
using FERCO.Model;

namespace FERCO.View
{
    public partial class InventarioDialog : Window
    {
        public UbicacionInventario? InventarioEditado { get; private set; }

        public InventarioDialog(UbicacionInventario? inventario = null)
        {
            InitializeComponent();

            if (inventario != null)
            {
                txtDescripcion.Text = inventario.Descripcion;
                InventarioEditado = inventario;
                this.Title = "Editar Ubicación";
            }
            else
            {
                InventarioEditado = new UbicacionInventario();
                this.Title = "Nueva Ubicación";
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                MessageBox.Show("Debe ingresar una descripción.");
                return;
            }

            InventarioEditado!.Descripcion = descripcion;

            bool exito;

            if (InventarioEditado.IdInventario == 0)
            {
                exito = InventarioDAO.AgregarUbicacion(InventarioEditado);
            }
            else
            {
                exito = InventarioDAO.ActualizarUbicacion(InventarioEditado);
            }

            if (exito)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Error al guardar la ubicación.");
            }
        }
    }
}
