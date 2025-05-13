using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LoginApp
{
    public partial class MenuPrincipal : Window
    {
        public MenuPrincipal()
        {
            InitializeComponent();
        }

        private void BtnIrAVentas_Click(object sender, RoutedEventArgs e)
        {
            VentanaVentas ventana = new VentanaVentas();
            ventana.Show();
            this.Close();
        }

        private void btnVentas_Click(object sender, RoutedEventArgs e)
        {
            VentanaVentas ventana = new VentanaVentas();
            ventana.Show();
            this.Close();
        }

        private void TB(object sender, RoutedEventArgs e)
        {

        }

        private void TBShow(object sender, RoutedEventArgs e)
        {
            GridContent.Opacity = 0.5;
        }

        private void TBHide(object sender, RoutedEventArgs e)
        {
            GridContent.Opacity = 1;
        }


        private void PreviewMouseLeftButtomDownBG(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnShowHide.IsChecked = false;
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Cerrar(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void BtnPOS_Click(object sender, RoutedEventArgs e)
        {
            VentanaVentas ventanaVentas = new VentanaVentas();
            ventanaVentas.ShowDialog();
        }
    }
}

