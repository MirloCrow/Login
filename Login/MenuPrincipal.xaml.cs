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
    }
}