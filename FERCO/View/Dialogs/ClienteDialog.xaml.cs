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

        private bool esEdicion = false;
        private Cliente clienteOriginal = new();

        public ClienteDialog(Cliente clienteExistente) : this()
        {
            esEdicion = true;
            clienteOriginal = clienteExistente;

            txtRut.Text = clienteExistente.RutCliente.ToString();
            txtNombre.Text = clienteExistente.NombreCliente;
            txtEmail.Text = clienteExistente.EmailCliente;
            txtDireccion.Text = clienteExistente.DireccionCliente;
            txtTelefono.Text = clienteExistente.TelefonoCliente.ToString();

            // Deshabilitar el campo RUT si no quieres que se edite
            txtRut.IsEnabled = false;

            this.Title = "Editar Cliente";
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

            if (!int.TryParse(telefonoStr, out int telefono))
                errores += "- El teléfono debe ser numérico.\n";

            if (!string.IsNullOrEmpty(errores))
            {
                MessageBox.Show(errores, "Errores de validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (esEdicion)
            {
                clienteOriginal.NombreCliente = nombre;
                clienteOriginal.EmailCliente = email;
                clienteOriginal.DireccionCliente = direccion;
                clienteOriginal.TelefonoCliente = (int)telefono;
                clienteOriginal.EstadoCliente = true;

                ClienteDAO.Actualizar(clienteOriginal);
                ClienteAgregado = clienteOriginal;
            }
            else
            {
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
            }

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