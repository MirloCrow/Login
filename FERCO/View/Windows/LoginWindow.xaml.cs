using System.Windows;
using System.Windows.Input;
using FERCO.ViewModel;

namespace FERCO
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtUsuario.Focus();
        }

        private void TxtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.Password = txtPassword.Password;
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && DataContext is LoginViewModel vm)
            {
                vm.LoginCommand.Execute(null);
            }
        }
    }
}
