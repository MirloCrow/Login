using System.Windows;
using FERCO.Model;
using FERCO.ViewModel;

namespace FERCO.View.Dialogs
{
    public partial class AgregarDetalleDialog : Window
    {
        public DetallePedido? Resultado { get; private set; }

        public AgregarDetalleDialog()
        {
            InitializeComponent();
        }

        private void Aceptar_Click(object sender, RoutedEventArgs e)
        {
            var vm = (AgregarDetalleDialogViewModel)DataContext;
            if (vm.ProductoSeleccionado == null || vm.Cantidad <= 0 || vm.PrecioUnitario <= 0)
            {
                MessageBox.Show("Debe seleccionar un producto y valores válidos.");
                return;
            }

            Resultado = new DetallePedido
            {
                IdProducto = vm.ProductoSeleccionado.IdProducto,
                NombreProducto = vm.ProductoSeleccionado.NombreProducto,
                Cantidad = vm.Cantidad,
                PrecioUnitario = vm.PrecioUnitario
            };

            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
