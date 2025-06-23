using FERCO.Data;
using FERCO.Model;
using System.Windows;
using System.Windows.Controls;

namespace FERCO.View.Controls
{
    public partial class SeguimientoReparacionesControl : UserControl
    {
        private bool _interaccionHabilitada = false;

        public SeguimientoReparacionesControl()
        {
            InitializeComponent();
            Loaded += (s, e) => _interaccionHabilitada = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // No ejecutar si todavía se está cargando la pantalla
            if (!_interaccionHabilitada)
                return;

            // Solo actuar si efectivamente se cambió un valor
            if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0)
                return;

            if (sender is ComboBox combo && combo.DataContext is ReparacionEditable reparacion)
            {
                bool ok = ReparacionDAO.ActualizarEstado(reparacion.IdReparacion, reparacion.Estado);
                if (ok)
                    MessageBox.Show("Estado actualizado correctamente.", "Confirmación", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Error al actualizar el estado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
