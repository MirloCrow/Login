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

        public void CargarUbicaciones(string nombreProducto, int idProducto)
        {
            NombreProducto = nombreProducto;

            var inventarios = InventarioDAO.ObtenerInventarios(); // List<Inventario>
            Ubicaciones.Clear();

            foreach (var inv in inventarios)
            {
                Ubicaciones.Add(new InventarioProducto
                {
                    IdInventario = inv.IdInventario,
                    IdProducto = idProducto,
                    Cantidad = 0,
                    DescripcionUbicacion = inv.Descripcion
                });
            }

            OnPropertyChanged(nameof(NombreProducto));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string nombre) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}
