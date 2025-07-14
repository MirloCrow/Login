using System;
using System.ComponentModel;

namespace FERCO.Model
{
    public class InventarioProducto : INotifyPropertyChanged
    {
        public int IdInventario { get; set; }
        public int IdProducto { get; set; }

        private int _cantidad;
        public int Cantidad
        {
            get => _cantidad;
            set
            {
                if (_cantidad != value)
                {
                    _cantidad = value;
                    OnPropertyChanged(nameof(Cantidad));
                    CantidadActualizada?.Invoke(); // Notifica al ViewModel
                }
            }
        }

        public string DescripcionUbicacion { get; set; } = "";

        // Callback que será asignado por el ViewModel
        public Action? CantidadActualizada { get; set; }

        public override string ToString()
        {
            return $"{DescripcionUbicacion} (Agregar: {Cantidad})";
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string nombre) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}
