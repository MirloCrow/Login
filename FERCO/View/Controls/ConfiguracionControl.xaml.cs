using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Controls;
using FERCO.Utilities;

namespace FERCO.View.Controls
{
    public partial class ConfiguracionControl : UserControl
    {
        public ConfiguracionControl()
        {
            InitializeComponent();
            CargarConfiguracion();
        }

        private void CargarConfiguracion()
        {
            int valorActual = ConfiguracionManager.Config.UmbralStockBajo;
            txtUmbral.Text = valorActual.ToString();
            lblUmbralTexto.Text = $"Umbral de stock bajo (valor actual: {valorActual}):";
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtUmbral.Text, out int nuevoUmbral) && nuevoUmbral >= 0)
            {
                ConfiguracionManager.Config.UmbralStockBajo = nuevoUmbral;
                ConfiguracionManager.GuardarConfiguracion();
                MessageBox.Show("Configuración guardada correctamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Ingrese un número válido para el umbral de stock bajo", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Text input

    [GeneratedRegex("^[0-9]+$")]
    private static partial Regex SoloNumerosRegex();

    private void TxtUmbral_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !SoloNumerosRegex().IsMatch(e.Text);
        }

        private void TxtUmbral_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string))!;
                if (!SoloNumerosRegex().IsMatch(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


    }
}