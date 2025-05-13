using System.Windows;
using Login.Data;
using Login.Model;

namespace Login
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string password = txtPassword.Password;

            var usuarioValido = UsuarioDAO.ObtenerPorCredenciales(usuario, password);

            if (usuarioValido != null && usuarioValido.Rol == "admin")
            {
                var menu = new MenuPrincipal();
                menu.Show();
                this.Close();
            }
            else
            {
                lblMensaje.Text = "Credenciales incorrectas.";
            }

        }
    }
}
