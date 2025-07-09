using FERCO.Data;
using FERCO.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FERCO.ViewModel
{
    public class AgregarDetalleDialogViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Producto> Productos { get; set; } = [];

        private Producto? _productoSeleccionado;
        public Producto? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set { _productoSeleccionado = value; OnPropertyChanged(nameof(ProductoSeleccionado)); }
        }

        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }

        public AgregarDetalleDialogViewModel()
        {
            var productos = ProductoDAO.ObtenerTodos();
            foreach (var p in productos)
                Productos.Add(p);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string nombre) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}
