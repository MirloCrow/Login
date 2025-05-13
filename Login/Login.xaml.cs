using System.Windows;
using Login.Data;
using Login.Model;

namespace LoginApp
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string password = txtPassword.Password;

            var usuarioValido = UsuarioDAO.ValidarLogin(usuario, password);

            if (usuarioValido != null && usuarioValido.Rol == "admin")
            {
                var menu = new MenuPrincipal(); // asegúrate de tener esta ventana
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
