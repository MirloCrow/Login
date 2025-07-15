using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FERCO.Utilities;

namespace FERCO.ViewModel
{
    public class ReporteVentasViewModel : INotifyPropertyChanged
    {
        private DateTime? _desde = DateTime.Today.AddDays(-7);
        public DateTime? Desde
        {
            get => _desde;
            set
            {
                if (_desde != value)
                {
                    _desde = value;
                    OnPropertyChanged();
                }
            }
        }

        private DateTime? _hasta = DateTime.Today;
        public DateTime? Hasta
        {
            get => _hasta;
            set
            {
                if (_hasta != value)
                {
                    _hasta = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Venta> Ventas { get; } = [];

        public ICommand FiltrarCommand { get; }

        public ReporteVentasViewModel()
        {
            FiltrarCommand = new RelayCommand(Filtrar);
            Filtrar(); // carga inicial
        }

        private void Filtrar()
        {
            var desde = Desde ?? DateTime.MinValue;
            var hasta = Hasta ?? DateTime.MaxValue;

            var resultado = ObtenerVentasDesdeBaseDeDatos()
                .Where(v => v.Fecha >= desde && v.Fecha <= hasta);

            Ventas.Clear();
            foreach (var venta in resultado)
                Ventas.Add(venta);
        }

        private List<Venta> ObtenerVentasDesdeBaseDeDatos()
        {
            return
            [
                new() { Id = 1, Fecha = DateTime.Today.AddDays(-3), Producto = "Zapato", Total = 10000 },
                new() { Id = 2, Fecha = DateTime.Today.AddDays(-1), Producto = "Bolso", Total = 20000 },
                new() { Id = 3, Fecha = DateTime.Today, Producto = "Chaqueta", Total = 25000 },
            ];
        }

        public class Venta
        {
            public int Id { get; set; }
            public DateTime Fecha { get; set; }
            public string Producto { get; set; } = string.Empty;
            public decimal Total { get; set; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
