using System.Windows;
using FERCO.Data;
using FERCO.Model;

namespace FERCO.View.Dialogs
{
    public partial class InventarioDialog : Window
    {
        public Inventario InventarioEditado { get; private set; }

        public InventarioDialog(Inventario? inventario = null)
        {
            InitializeComponent();

            if (inventario != null)
            {
                txtDescripcion.Text = inventario.Descripcion;
                InventarioEditado = inventario;
                Title = "Editar Inventario";
            }
            else
            {
                InventarioEditado = new Inventario();
                Title = "Nuevo Inventario";
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(descripcion))
            {
                MessageBox.Show("La descripción no puede estar vacía.");
                return;
            }

            InventarioEditado.Descripcion = descripcion;

            bool exito;
            if (InventarioEditado.IdInventario == 0)
            {
                exito = InventarioDAO.Agregar(InventarioEditado);
                InventarioEditado = InventarioDAO.ObtenerUltimo()!;
            }
            else
            {
                exito = InventarioDAO.Actualizar(InventarioEditado);
            }

            if (exito)
            {
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Error al guardar el inventario.");
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

    }
}
