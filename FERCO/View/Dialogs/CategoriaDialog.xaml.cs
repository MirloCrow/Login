using System.Windows;
using FERCO.Model;
using FERCO.Data;

namespace FERCO.View.Dialogs
{
    public partial class CategoriaDialog : Window
    {
        public Categoria CategoriaEditada { get; private set; }

        public CategoriaDialog(Categoria? categoria = null)
        {
            InitializeComponent();

            if (categoria != null)
            {
                txtNombre.Text = categoria.Nombre;
                txtDescripcion.Text = categoria.Descripcion;
                CategoriaEditada = categoria;
                this.Title = "Editar Categoría";
            }
            else
            {
                CategoriaEditada = new Categoria();
                this.Title = "Nueva Categoría";
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            CategoriaEditada.Nombre = txtNombre.Text.Trim();
            CategoriaEditada.Descripcion = txtDescripcion.Text.Trim();

            if (string.IsNullOrWhiteSpace(CategoriaEditada.Nombre))
            {
                MessageBox.Show("El nombre no puede estar vacío.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool resultado;
            if (CategoriaEditada.IdCategoria == 0)
            {
                resultado = CategoriaDAO.Agregar(CategoriaEditada);
                CategoriaEditada = CategoriaDAO.ObtenerUltima()!;
            }
            else
            {
                resultado = CategoriaDAO.Actualizar(CategoriaEditada);
            }

            if (resultado)
            {
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("No se pudo guardar la categoría.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
