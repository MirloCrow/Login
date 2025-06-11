using System;
using System.Windows;
using FERCO.Model;
using FERCO.Data;

namespace FERCO.View.Dialogs
{
    public partial class ClienteDialog : Window
    {
        public Cliente? ClienteAgregado { get; private set; }

        public ClienteDialog()
        {
            InitializeComponent();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            string errores = "";

            string nombre = txtNombre.Text.Trim();
            string email = txtEmail.Text.Trim();
            string direccion = txtDireccion.Text.Trim();
            string telefonoStr = txtTelefono.Text.Trim();
            string rutStr = txtRut.Text.Trim();

            if (!int.TryParse(rutStr, out int rut))
                errores += "- El RUT debe ser numérico (sin puntos ni DV).\n";

            if (string.IsNullOrWhiteSpace(nombre))
                errores += "- El nombre no puede estar vacío.\n";

            if (!long.TryParse(telefonoStr, out long telefono))
                errores += "- El teléfono debe ser numérico.\n";

            if (!string.IsNullOrEmpty(errores))
            {
                MessageBox.Show(errores, "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Cliente nuevo = new()
            {
                RutCliente = rut,
                NombreCliente = nombre,
                EmailCliente = email,
                DireccionCliente = direccion,
                TelefonoCliente = (int)telefono,
                EstadoCliente = false
            };

            int id = ClienteDAO.AgregarYObtenerId(nuevo);
            nuevo.IdCliente = id;

            ClienteAgregado = nuevo;
            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}