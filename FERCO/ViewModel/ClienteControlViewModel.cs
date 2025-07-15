using FERCO.Data;
using FERCO.Model;
using FERCO.Utilities;
using FERCO.View.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FERCO.ViewModel
{
    public class ClienteControlViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Cliente> ListaClientes { get; set; } = [];

        private Cliente? _clienteSeleccionado;
        public Cliente? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set
            {
                _clienteSeleccionado = value;
                OnPropertyChanged();
                CargarHistorialCliente();
            }
        }

        private string _textoBusqueda = "";
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged();
            }
        }

        public ICommand BuscarCommand { get; }
        public ICommand NuevoClienteCommand { get; }
        public ICommand EditarClienteCommand { get; }

        public ClienteControlViewModel()
        {
            BuscarCommand = new RelayCommand(() => BuscarClientes());
            NuevoClienteCommand = new RelayCommand(() => AbrirDialogoNuevoCliente());
            EditarClienteCommand = new RelayCommand(() => AbrirDialogoEditarCliente(), () => ClienteSeleccionado != null);

            CargarClientes();
        }

        private void CargarClientes()
        {
            ListaClientes.Clear();
            var clientes = ClienteDAO.ObtenerTodos();
            foreach (var cliente in clientes)
            {
                ListaClientes.Add(cliente);
            }
        }

        private void BuscarClientes()
        {
            ListaClientes.Clear();
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                CargarClientes();
                return;
            }

            var cliente = ClienteDAO.BuscarPorNombre(TextoBusqueda.Trim());
            if (cliente != null)
            {
                ListaClientes.Add(cliente);
            }
            else
            {
                MessageBox.Show("No se encontraron resultados.");
            }
        }

        private void AbrirDialogoNuevoCliente()
        {
            var dialog = new ClienteDialog(); // Asume que este dialog tiene DataContext por defecto
            if (dialog.ShowDialog() == true)
            {
                CargarClientes();
            }
        }

        private void AbrirDialogoEditarCliente()
        {
            if (ClienteSeleccionado == null) return;

            var dialog = new ClienteDialog(ClienteSeleccionado);
            if (dialog.ShowDialog() == true)
            {
                CargarClientes();
            }
        }

        private void CargarHistorialCliente()
        {
            if (ClienteSeleccionado == null) return;

            // Aquí se debe integrar ReparacionDAO y VentaDAO
            ClienteSeleccionado.Reparaciones = ReparacionDAO.ObtenerPorCliente(ClienteSeleccionado.IdCliente);
            ClienteSeleccionado.Ventas = VentaDAO.ObtenerPorCliente(ClienteSeleccionado.IdCliente);

            // Notificar cambios en historial
            OnPropertyChanged(nameof(ClienteSeleccionado));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
