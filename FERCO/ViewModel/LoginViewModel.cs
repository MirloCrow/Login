using FERCO.Data;
using FERCO.Model;
using FERCO.Utilities;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FERCO.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _usuario = string.Empty;
        private string _password = string.Empty;
        private string _mensaje = string.Empty;

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

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password))
                {
                    Mensaje = "Por favor, ingrese usuario y contraseña.";
                    return;
                }

                // Aplica hash a la contraseña ingresada
                string passwordHasheada = SecurityHelper.HashearSHA256(Password);

                // Verifica credenciales con contraseña hasheada
                var usuarioValido = UsuarioDAO.ObtenerPorCredenciales(Usuario, passwordHasheada);

                if (usuarioValido != null)
                {
                    // Crea e inicia MainWindow
                    var main = new MainWindow();
                    Application.Current.MainWindow = main;
                    main.Show();

                    // Cierra LoginWindow
                    foreach (Window w in Application.Current.Windows)
                    {
                        if (w is Window win && win.GetType().Name == "LoginWindow")
                        {
                            win.Close();
                            break;
                        }
                    }
                }
                else
                {
                    Mensaje = "Credenciales incorrectas.";
                }
            }
            catch (Exception ex)
            {
                Mensaje = "Error inesperado al iniciar sesión.";
                Console.WriteLine($"[LoginViewModel] Error: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
