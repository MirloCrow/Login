using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FERCO.Model;
using FERCO.Data;
using FERCO.Utilities;
using System.Linq;
using System.Windows;

namespace FERCO.ViewModel
{
    public class ReparacionViewModel : INotifyPropertyChanged
    {
        // Clientes
        public ObservableCollection<Cliente> Clientes { get; set; } = [];
        private Cliente? _clienteSeleccionado;
        public Cliente? ClienteSeleccionado
        {
            get => _clienteSeleccionado;
            set { _clienteSeleccionado = value; OnPropertyChanged(); }
        }

        // Tipos de reparación
        public ObservableCollection<TipoReparacion> TiposReparacion { get; set; } = [];
        private TipoReparacion? _tipoSeleccionado;
        public TipoReparacion? TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                _tipoSeleccionado = value;
                OnPropertyChanged();
                CargarProductosParaTipo();
            }
        }

        // Tipos
        public ICommand AgregarTipoReparacionCommand { get; set; }
        private void AgregarTipoReparacion()
        {
            var dialog = new FERCO.View.Dialogs.TipoReparacionDialog();
            if (dialog.ShowDialog() == true)
            {
                CargarTiposReparacion(); // actualiza lista después de agregar
                MessageBox.Show("Tipo de reparación agregado correctamente.");
            }
        }


        // Productos a usar en la reparación
        public ObservableCollection<TipoReparacionProductoEditable> ProductosReparacion { get; set; } = [];

        // Comandos
        public ICommand AgregarClienteCommand { get; set; }
        public ICommand AgendarReparacionCommand { get; set; }

        public ReparacionViewModel()
        {
            AgregarClienteCommand = new RelayCommand(AgregarCliente);
            AgendarReparacionCommand = new RelayCommand(AgendarReparacion);
            AgregarTipoReparacionCommand = new RelayCommand(AgregarTipoReparacion);

            CargarClientes();
            CargarTiposReparacion();
        }

        private void CargarClientes()
        {
            var lista = ClienteDAO.ObtenerTodos();
            Clientes.Clear();
            foreach (var c in lista)
                Clientes.Add(c);
        }

        private void CargarTiposReparacion()
        {
            var lista = TipoReparacionDAO.ObtenerTodas();
            TiposReparacion.Clear();
            foreach (var t in lista)
                TiposReparacion.Add(t);
        }

        private void CargarProductosParaTipo()
        {
            ProductosReparacion.Clear();
            if (TipoSeleccionado == null) return;

            var productos = TipoReparacionProductoDAO.ObtenerProductosPorTipo(TipoSeleccionado.IdTipoReparacion);
            foreach (var p in productos)
            {
                ProductosReparacion.Add(new TipoReparacionProductoEditable
                {
                    IdProducto = p.IdProducto,
                    NombreProducto = p.NombreProducto,
                    CantidadRequerida = p.CantidadRequerida,
                    CantidadAUsar = p.CantidadRequerida
                });
            }
        }

        private void AgregarCliente()
        {
            var dialog = new FERCO.View.Dialogs.ClienteDialog(); // Asegúrate que esté en esta ruta
            dialog.ShowDialog();
            CargarClientes();
        }

        private void AgendarReparacion()
        {
            if (ClienteSeleccionado == null || TipoSeleccionado == null || ProductosReparacion.Count == 0)
            {
                MessageBox.Show("Debes seleccionar un cliente, un tipo de reparación y al menos un producto.", "Advertencia");
                return;
            }

            bool exito = ReparacionDAO.CrearReparacionConDetalle(
                ClienteSeleccionado.IdCliente,
                TipoSeleccionado.IdTipoReparacion,
                [.. ProductosReparacion],
                out string mensajeError
            );

            if (exito)
            {
                MessageBox.Show("Reparación agendada exitosamente.");
            }
            else
            {
                MessageBox.Show($"Error al agendar: {mensajeError}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public class TipoReparacionProductoEditable : TipoReparacionProducto, INotifyPropertyChanged
    {
        private int _cantidadAUsar;
        public int CantidadAUsar
        {
            get => _cantidadAUsar;
            set
            {
                _cantidadAUsar = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StockSuficiente));
            }
        }

        public int StockDisponible => InventarioProductoDAO.ObtenerStockTotal(IdProducto);

        public bool StockSuficiente => StockDisponible >= CantidadAUsar;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}
