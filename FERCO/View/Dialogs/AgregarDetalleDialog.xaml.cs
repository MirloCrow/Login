using System.Windows;
using System.Windows.Input;
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

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            txtBuscar.Focus();
            Keyboard.Focus(txtBuscar);
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
