using FERCO.Data;
using FERCO.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FERCO.ViewModel
{
    public class FirstTimeSetupViewModel : INotifyPropertyChanged
    {
        private string _usuario = string.Empty;
        private string _password = string.Empty;
        private string _mensaje = string.Empty;
        private string _confirmarPassword = string.Empty;
        private bool _verPassword;
        private bool _verConfirmarPassword;

        public string Usuario
        {
            get => _usuario;
            set { _usuario = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public string Mensaje
        {
            get => _mensaje;
            set { _mensaje = value; OnPropertyChanged(); }
        }

        public string ConfirmarPassword
        {
            get => _confirmarPassword;
            set { _confirmarPassword = value; OnPropertyChanged(); }
        }
        public bool VerPassword
        {
            get => _verPassword;
            set { _verPassword = value; OnPropertyChanged(); }
        }
        public bool VerConfirmarPassword
        {
            get => _verConfirmarPassword;
            set { _verConfirmarPassword = value; OnPropertyChanged(); }
        }

        public ICommand CrearUsuarioCommand { get; }

        public FirstTimeSetupViewModel()
        {
            CrearUsuarioCommand = new RelayCommand(CrearUsuario);
        }

        private void CrearUsuario()
        {
            Mensaje = string.Empty;

            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmarPassword))
            {
                Mensaje = "Se requiere usuario, contraseña y confirmación";
                return;
            }

            if (Password != ConfirmarPassword)
            {
                Mensaje = "Las contraseñas no coinciden";
                return;
            }

            try
            {
                if (UsuarioDAO.ExisteAlMenosUnUsuario())
                {
                    Mensaje = "Ya existe un usuario. Este setup se permite solo una vez";
                    return;
                }

                UsuarioDAO.AgregarUsuario(Usuario, Password, "admin");

                MessageBox.Show("Cuenta de administrador creada con éxito.", "FERCO", MessageBoxButton.OK, MessageBoxImage.Information);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var login = new LoginWindow();
                    Application.Current.MainWindow = login;
                    login.Show();

                    foreach (Window w in Application.Current.Windows)
                    {
                        if (w.GetType().Name == "FirstTimeSetupWindow")
                        {
                            w.Close();
                            break;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Mensaje = $"Error: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
