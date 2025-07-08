using FERCO.Data;
using FERCO.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FERCO.View.Dialogs
{
    public partial class ProductoDialog : Window
    {
        public Producto? ProductoEditado { get; private set; }

        private readonly bool esEdicion;

        public ProductoDialog(Producto? producto = null)
        {
            InitializeComponent();
            esEdicion = producto != null;
            ProductoEditado = producto != null ? new Producto
            {
                IdProducto = producto.IdProducto,
                CodigoProducto = producto.CodigoProducto,
                NombreProducto = producto.NombreProducto,
                DescripcionProducto = producto.DescripcionProducto,
                PrecioProducto = producto.PrecioProducto,
                IdCategoria = producto.IdCategoria,
                IdProveedor = producto.IdProveedor
            } : new Producto();

            CargarCategorias();
            CargarProveedores();

            if (esEdicion)
            {
                Title = "Editar Producto";
                txtNombre.Text = ProductoEditado!.NombreProducto;
                txtDescripcion.Text = ProductoEditado.DescripcionProducto;
                txtPrecio.Text = ProductoEditado.PrecioProducto.ToString();
                txtCodigo.Text = ProductoEditado.CodigoProducto;
                cmbCategoria.SelectedValue = ProductoEditado.IdCategoria;
                cmbProveedor.SelectedValue = ProductoEditado.IdProveedor;
            }
        }

        private void CargarCategorias()
        {
            cmbCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
        }

        private void CargarProveedores()
        {
            cmbProveedor.ItemsSource = ProveedorDAO.ObtenerTodos();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarCampos()) return;

            ProductoEditado!.NombreProducto = txtNombre.Text.Trim();
            ProductoEditado.DescripcionProducto = txtDescripcion.Text.Trim();
            ProductoEditado.CodigoProducto = txtCodigo.Text.Trim();
            ProductoEditado.PrecioProducto = int.Parse(txtPrecio.Text.Trim());
            ProductoEditado.IdCategoria = (int)cmbCategoria.SelectedValue;
            ProductoEditado.IdProveedor = (int)cmbProveedor.SelectedValue;

            bool exito = esEdicion
                ? ProductoDAO.Actualizar(ProductoEditado)
                : ProductoDAO.Agregar(ProductoEditado);

            if (!exito)
            {
                MessageBox.Show("Error al guardar el producto. Verifica los datos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
            Close();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                string.IsNullOrWhiteSpace(txtCodigo.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                cmbCategoria.SelectedItem == null ||
                cmbProveedor.SelectedItem == null)
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtPrecio.Text.Trim(), out int precio) || precio < 0)
            {
                MessageBox.Show("El precio debe ser un número entero positivo.", "Validación", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}
