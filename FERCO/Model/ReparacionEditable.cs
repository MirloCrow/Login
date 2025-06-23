using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FERCO.Model
{
    public class ReparacionEditable : INotifyPropertyChanged
    {
        public int IdReparacion { get; set; }
        public string NombreCliente { get; set; } = "";
        public DateTime Fecha { get; set; }
        public int Costo { get; set; }

        private string _estado = "";
        public string Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? prop = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
