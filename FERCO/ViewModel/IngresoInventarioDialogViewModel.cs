using FERCO.Data;
using FERCO.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FERCO.ViewModel
{
    public class IngresoInventarioDialogViewModel : INotifyPropertyChanged
    {
        public string NombreProducto { get; set; } = "";

        public ObservableCollection<InventarioProducto> Ubicaciones { get; set; } = [];

        private int _cantidadEsperada;
        public int CantidadEsperada
        {
            get => _cantidadEsperada;
            set
            {
                _cantidadEsperada = value;
                OnPropertyChanged(nameof(CantidadEsperada));
                OnPropertyChanged(nameof(ResumenAsignacion));
                OnPropertyChanged(nameof(PuedeConfirmar));
            }
        }

        public int TotalAsignado => Ubicaciones.Sum(u => u.Cantidad);

        public string ResumenAsignacion =>
            $"Total por asignar: {CantidadEsperada} - Asignado: {TotalAsignado} - Restantes: {CantidadEsperada - TotalAsignado}";

        public bool PuedeConfirmar => TotalAsignado == CantidadEsperada;

        public void CargarUbicaciones(string nombreProducto, int idProducto, int cantidadEsperada)
        {
            NombreProducto = nombreProducto;
            CantidadEsperada = cantidadEsperada;

            var inventarios = InventarioDAO.ObtenerInventarios();
            Ubicaciones.Clear();

            foreach (var inv in inventarios)
            {
                var item = new InventarioProducto
                {
                    IdInventario = inv.IdInventario,
                    IdProducto = idProducto,
                    Cantidad = 0,
                    DescripcionUbicacion = inv.Descripcion
                };

                // Suscribirse al evento de cambio
                item.PropertyChanged += Ubicacion_PropertyChanged;

                Ubicaciones.Add(item);
            }

            OnPropertyChanged(nameof(NombreProducto));
        }
        private void Ubicacion_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InventarioProducto.Cantidad))
            {
                OnPropertyChanged(nameof(TotalAsignado));
                OnPropertyChanged(nameof(ResumenAsignacion));
                OnPropertyChanged(nameof(PuedeConfirmar));
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string nombre) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}
