using FERCO.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace FERCO.View.Windows
{
    public partial class FirstTimeSetupWindow : Window
    {
        public FirstTimeSetupWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is FirstTimeSetupViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is FirstTimeSetupViewModel vm)
            {
                vm.ConfirmarPassword = ((PasswordBox)sender).Password;
            }
        }
        private void CerrarVentana_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}