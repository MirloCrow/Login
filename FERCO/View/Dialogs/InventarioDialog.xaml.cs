using System.Windows;
using FERCO.Data;
using FERCO.Model;

namespace FERCO.View
{
    public partial class InventarioDialog : Window
    {
        public Inventario InventarioEditado { get; private set; }

        public InventarioDialog(Inventario? inventario = null)
        {
            InitializeComponent();

            if (inventario != null)
            {
                txtIdProducto.Text = inventario.IdProducto.ToString();
                txtCantidad.Text = inventario.CantidadProducto.ToString();
                InventarioEditado = inventario;
                this.Title = "Editar Inventario";
            }
            else
            {
                InventarioEditado = new Inventario();
                this.Title = "Nuevo Inventario";
            }
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtIdProducto.Text, out int idProd) || idProd <= 0)
            {
                MessageBox.Show("ID de producto inválido.");
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad < 0)
            {
                MessageBox.Show("Cantidad inválida.");
                return;
            }

            InventarioEditado.IdProducto = idProd;
            InventarioEditado.CantidadProducto = cantidad;

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
    }
}
