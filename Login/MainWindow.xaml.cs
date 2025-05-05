using System.Data.SQLite;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;






namespace LoginApp
{
    public partial class MainWindow : Window
    {
        string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "usuarios.db");

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string usuario = txtUsuario.Text;
            string password = txtPassword.Password;

            if (ValidarUsuario(usuario, password))
            {
                MenuPrincipal menu = new MenuPrincipal();
                menu.Show();
                this.Close();
            }
            else
            {
                lblMensaje.Text = "Credenciales incorrectas.";
            }
        }

        private bool ValidarUsuario(string usuario, string password)
        {
            if (!File.Exists(dbPath)) return false;

            using (SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM usuarios WHERE nombre = @nombre AND password = @password";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nombre", usuario);
                    cmd.Parameters.AddWithValue("@password", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}