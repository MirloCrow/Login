using FERCO.Data;
using FERCO.Model;
using FERCO.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace FERCO.ViewModel
{
    public class SeguimientoReparacionesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ReparacionEditable> Reparaciones { get; set; } = [];
        public ObservableCollection<string> EstadosFiltro { get; set; } = ["Todos", "Pendiente", "En proceso", "Completada"];
        public ObservableCollection<string> EstadosEdicion { get; set; } = ["Pendiente", "En proceso", "Completada"];


        private string _estadoSeleccionado = "Todos";
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                _estadoSeleccionado = value;
                OnPropertyChanged();
                FiltrarReparaciones();
            }
        }

        public ICommand GuardarEstadoCommand { get; set; }

        public SeguimientoReparacionesViewModel()
        {
            GuardarEstadoCommand = new RelayCommand<ReparacionEditable>(GuardarEstado);
            CargarReparaciones();
        }

        private List<ReparacionEditable> todasLasReparaciones = [];

        private void CargarReparaciones()
        {
            todasLasReparaciones = ReparacionDAO.ObtenerTodas();
            FiltrarReparaciones();
        }

        private void FiltrarReparaciones()
        {
            Reparaciones.Clear();
            var filtradas = EstadoSeleccionado == "Todos"
                ? todasLasReparaciones
                : todasLasReparaciones.Where(r => r.Estado == EstadoSeleccionado);

            foreach (var r in filtradas)
                Reparaciones.Add(r);
        }

        private void GuardarEstado(ReparacionEditable reparacion)
        {
            if (ReparacionDAO.ActualizarEstado(reparacion.IdReparacion, reparacion.Estado))
            {
                MessageBox.Show("Estado actualizado correctamente.");
            }
            else
            {
                MessageBox.Show("Error al actualizar el estado.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
