using FERCO.Data;
using FERCO.Model;
using FERCO.Utilities;
using FERCO.View.Dialogs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
                BuscarClientes();
            }
        }

        public ObservableCollection<string> OpcionesBusqueda { get; } = ["Nombre", "RUT"];

        private string _tipoBusqueda = "Nombre";
        public string TipoBusqueda
        {
            get => _tipoBusqueda;
            set
            {
                _tipoBusqueda = value;
                OnPropertyChanged();
            }
        }

        private Venta? _ventaSeleccionada;
        public Venta? VentaSeleccionada
        {
            get => _ventaSeleccionada;
            set
            {
                _ventaSeleccionada = value;
                OnPropertyChanged();
                CargarDetalleVenta();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private Reparacion? _reparacionSeleccionada;
        public Reparacion? ReparacionSeleccionada
        {
            get => _reparacionSeleccionada;
            set
            {
                _reparacionSeleccionada = value;
                OnPropertyChanged();
                CargarDetalleReparacion();
            }
        }

        public ObservableCollection<DetalleVenta> DetallesVenta { get; set; } = [];
        public ObservableCollection<DetalleReparacion> DetallesReparacion { get; set; } = [];



        public ICommand BuscarCommand { get; }
        public ICommand NuevoClienteCommand { get; }
        public ICommand EditarClienteCommand { get; }
        public ICommand VerBoletaCommand { get; }
        public ICommand VerOrdenServicioCommand { get; }


        public ClienteControlViewModel()
        {
            BuscarCommand = new RelayCommand(() => BuscarClientes());
            NuevoClienteCommand = new RelayCommand(() => AbrirDialogoNuevoCliente());
            EditarClienteCommand = new RelayCommand(() => AbrirDialogoEditarCliente(), () => ClienteSeleccionado != null);
            VerBoletaCommand = new RelayCommand(VerBoleta, () => VentaSeleccionada != null || ReparacionSeleccionada != null);
            VerOrdenServicioCommand = new RelayCommand(VerOrdenServicio, () => ReparacionSeleccionada != null);
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

            var resultados = ClienteDAO.BuscarPorCriterio(TextoBusqueda.Trim());

            if (resultados.Count > 0)
            {
                foreach (var c in resultados)
                    ListaClientes.Add(c);
            }
            else
            {
                // Opcional: no mostrar mensaje si quieres evitar interrupciones
                // MessageBox.Show("No se encontraron resultados.");
            }
        }


        private void AbrirDialogoNuevoCliente()
        {
            var dialog = new ClienteDialog();
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

        private void CargarDetalleVenta()
        {
            DetallesVenta.Clear();
            if (VentaSeleccionada != null)
            {
                var detalles = VentaDAO.ObtenerDetalles(VentaSeleccionada.IdVenta);
                foreach (var d in detalles)
                    DetallesVenta.Add(d);
            }
        }

        private void CargarDetalleReparacion()
        {
            DetallesReparacion.Clear();
            if (ReparacionSeleccionada != null)
            {
                var detalles = ReparacionDAO.ObtenerDetalles(ReparacionSeleccionada.IdReparacion);
                foreach (var d in detalles)
                    DetallesReparacion.Add(d);
            }
        }
        private void VerOrdenServicio()
        {
            if (ReparacionSeleccionada == null) return;

            try
            {
                var detalles = ReparacionDAO.ObtenerDetalles(ReparacionSeleccionada.IdReparacion);

                // Asegurarse de tener el nombre del cliente antes de generar la boleta
                ReparacionSeleccionada.NombreCliente = ClienteSeleccionado?.NombreCliente ?? "Cliente sin nombre";

                BoletaHTMLGenerator.GenerarBoleta(ReparacionSeleccionada, detalles);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar orden de servicio: " + ex.Message);
            }
        }

        private void VerBoleta()
        {
            try
            {
                string rutaArchivo = "";

                if (VentaSeleccionada != null)
                {
                    var detalles = VentaDAO.ObtenerDetalles(VentaSeleccionada.IdVenta);
                    BoletaHTMLGenerator.GenerarBoleta(VentaSeleccionada, detalles);
                    rutaArchivo = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        "BoletasFERCO",
                        $"Boleta_{VentaSeleccionada.IdVenta}.html"
                    );
                }
                else if (ReparacionSeleccionada != null && ClienteSeleccionado != null)
                {
                    var detalles = ReparacionDAO.ObtenerDetalles(ReparacionSeleccionada.IdReparacion);
                    BoletaHTMLGenerator.GenerarBoleta(ReparacionSeleccionada, [.. detalles]);
                    rutaArchivo = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        "BoletasFERCO",
                        $"BoletaReparacion_{ReparacionSeleccionada.IdReparacion}.html"
                    );
                }
                else
                {
                    MessageBox.Show("Debe seleccionar una venta o reparación para generar la boleta.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show($"Boleta guardada correctamente en:\n{rutaArchivo}", "Boleta Generada", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la boleta: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
