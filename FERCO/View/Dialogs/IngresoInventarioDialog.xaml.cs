using FERCO.Model;
using FERCO.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace FERCO.View.Dialogs
{
    public partial class IngresoInventarioDialog : Window
    {
        public List<InventarioProducto> Resultado { get; private set; } = [];

        private readonly int cantidadEsperada;

        public IngresoInventarioDialog(string nombreProducto, int cantidad, int idProducto)
        {
            InitializeComponent();
            cantidadEsperada = cantidad;

            if (DataContext is IngresoInventarioDialogViewModel vm)
                vm.CargarUbicaciones(nombreProducto, idProducto, cantidad);
        }

        private void Confirmar_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not IngresoInventarioDialogViewModel vm)
                return;

            int total = vm.Ubicaciones.Sum(u => u.Cantidad);

            if (total != cantidadEsperada)
            {
                MessageBox.Show($"Debe asignar exactamente {cantidadEsperada} unidades entre las ubicaciones.", "Cantidad incorrecta", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Resultado = [.. vm.Ubicaciones.Where(u => u.Cantidad > 0)];
            DialogResult = true;
            Close();
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void SoloNumerosHandler(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }
        private void SoloNumerosPegarHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string textoPegado = (string)e.DataObject.GetData(typeof(string));
                if (!int.TryParse(textoPegado, out _))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }


    }
}
