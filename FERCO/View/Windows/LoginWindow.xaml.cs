using System.Windows;
using FERCO.Data;
using FERCO.Model;

namespace FERCO
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
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
                var menu = new MainWindow();
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
