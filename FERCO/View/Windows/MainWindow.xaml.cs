using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using FERCO.View;
using FERCO.View.Controls;
using FERCO;

namespace FERCO
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TB(object sender, RoutedEventArgs e)
        {

        }

        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new TextBlock
            {
                Text = "TallerFERCO",
                FontSize = 50,
                Foreground = (Brush)FindResource("PrimaryTextBrush"),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private void BtnProducto_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new ProductoControl();
        }
        private void BtnInventario_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new InventarioControl();
        }

        private void BtnProveedores_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new ProveedorControl();
        }

        private void BtnPOS_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new VentaControl();
        }
        private void BtnSeguimiento_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new SeguimientoReparacionesControl();
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


        private void BtnShowHide_Checked(object sender, RoutedEventArgs e)
        {
            var expand = (Storyboard)FindResource("ExpandSidebar");
            expand.Begin();
        }

        private void BtnShowHide_Unchecked(object sender, RoutedEventArgs e)
        {
            var collapse = (Storyboard)FindResource("CollapseSidebar");
            collapse.Begin();
        }

        private void Minimizar(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Cerrar(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void BtnReparaciones_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new ReparacionControl();
        }
        private void BtnConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new ConfiguracionControl();
        }
        private void BtnCompras_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new CompraControl();
        }
        private void BtnCliente_Click(object sender, RoutedEventArgs e)
        {
            ContenidoPrincipal.Content = new ClienteControl();
        }
    }
}