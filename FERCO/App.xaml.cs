using FERCO.Data;
using FERCO.View.Windows;
using System.Windows;

namespace FERCO
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (UsuarioDAO.ExisteAlMenosUnUsuario())
            {
                var login = new LoginWindow();
                MainWindow = login;
                login.Show();
            }
            else
            {
                var setup = new FirstTimeSetupWindow();
                MainWindow = setup;
                setup.Show();
            }
        }
    }
}