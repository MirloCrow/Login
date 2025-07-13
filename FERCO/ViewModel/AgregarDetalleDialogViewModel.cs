using FERCO.Data;
using FERCO.Model;
using FERCO.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace FERCO.ViewModel
{
    public class AgregarDetalleDialogViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Producto> Productos { get; set; } = new();
        public ObservableCollection<Producto> ProductosFiltrados { get; set; } = new();
        public ObservableCollection<Categoria> Categorias { get; set; } = new();

        private Producto? _productoSeleccionado;
        public Producto? ProductoSeleccionado
        {
            get => _productoSeleccionado;
            set { _productoSeleccionado = value; OnPropertyChanged(nameof(ProductoSeleccionado)); }
        }

        private string _textoBusqueda = string.Empty;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged(nameof(TextoBusqueda));
                if (!EsFiltroPorCategoria)
                    FiltrarProductos();
            }
        }

        private string _criterioBusqueda = "Nombre";
        public string CriterioBusqueda
        {
            get => _criterioBusqueda;
            set
            {
                _criterioBusqueda = value;
                OnPropertyChanged(nameof(CriterioBusqueda));
                OnPropertyChanged(nameof(EsFiltroPorCategoria));
                FiltrarProductos();
            }
        }

        private Categoria? _categoriaSeleccionada;
        public Categoria? CategoriaSeleccionada
        {
            get => _categoriaSeleccionada;
            set
            {
                _categoriaSeleccionada = value;
                OnPropertyChanged(nameof(CategoriaSeleccionada));
                if (EsFiltroPorCategoria)
                    FiltrarProductos();
            }
        }

        private bool _ordenAscendente = true;
        public bool OrdenAscendente
        {
            get => _ordenAscendente;
            set
            {
                _ordenAscendente = value;
                OnPropertyChanged(nameof(OrdenAscendente));
                if (CriterioBusqueda == "Nombre")
                    FiltrarProductos();
            }
        }

        public bool EsFiltroPorCategoria => CriterioBusqueda == "Categoría";

        public int Cantidad { get; set; }
        public int PrecioUnitario { get; set; }

        public ICommand BuscarCommand { get; set; }

        public AgregarDetalleDialogViewModel()
        {
            BuscarCommand = new RelayCommand(FiltrarProductos);

            var productos = ProductoDAO.ObtenerTodos();
            foreach (var p in productos)
                Productos.Add(p);

            var categorias = CategoriaDAO.ObtenerTodas();
            foreach (var c in categorias)
                Categorias.Add(c);

            FiltrarProductos();
        }

        private void FiltrarProductos()
        {
            var filtrados = Productos.AsEnumerable();

            if (CriterioBusqueda == "Nombre")
            {
                string texto = TextoBusqueda.Trim().ToLower();
                filtrados = filtrados.Where(p => p.NombreProducto?.ToLower().Contains(texto) == true);

                filtrados = OrdenAscendente
                    ? filtrados.OrderBy(p => p.NombreProducto)
                    : filtrados.OrderByDescending(p => p.NombreProducto);
            }
            else if (CriterioBusqueda == "Código")
            {
                string texto = TextoBusqueda.Replace("-", "").Trim().ToLower();
                filtrados = filtrados.Where(p =>
                    p.CodigoProducto?.Replace("-", "").ToLower().Contains(texto) == true);
            }
            else if (CriterioBusqueda == "Categoría" && CategoriaSeleccionada != null)
            {
                filtrados = filtrados.Where(p => p.NombreCategoria == CategoriaSeleccionada.Nombre);
            }

            ProductosFiltrados.Clear();
            foreach (var p in filtrados)
                ProductosFiltrados.Add(p);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string nombre) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }
}
