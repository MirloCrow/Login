using FERCO.View.Dialogs;
using FERCO.ViewModel;
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

namespace FERCO.View.Controls
{
    /// <summary>
    /// Interaction logic for CompraControl.xaml
    /// </summary>
    public partial class CompraControl : UserControl
    {
        public CompraControl()
        {
            InitializeComponent();
        }
        private void BtnAgregarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProveedorDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                if (DataContext is CompraViewModel vm)
                {
                    var nuevoProveedor = dialog.ProveedorEditado;
                    vm.Proveedores.Add(nuevoProveedor);
                    vm.ProveedorSeleccionado = nuevoProveedor;
                }
            }
        }

    }
}
