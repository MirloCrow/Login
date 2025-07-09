using FERCO.Model;
using FERCO.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
                vm.CargarUbicaciones(nombreProducto, idProducto);
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
    }
}
