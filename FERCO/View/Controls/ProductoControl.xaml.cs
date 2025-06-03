using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FERCO.Data;
using FERCO.Model;

namespace FERCO.View
{
    public partial class ProductoControl : UserControl
    {
        private Producto? productoSeleccionado = null;

        public ProductoControl()
        {
            InitializeComponent();
            CargarCombos();
            CargarProductos();
        }

        // LOADERS
        private void CargarCombos()
        {
            cmbProveedor.ItemsSource = ProveedorDAO.ObtenerTodos();
            cmbProveedor.DisplayMemberPath = "Nombre";
            cmbProveedor.SelectedValuePath = "IdProveedor";

            cmbCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
            cmbCategoria.DisplayMemberPath = "Nombre";
            cmbCategoria.SelectedValuePath = "IdCategoria";

            cmbInventario.ItemsSource = InventarioDAO.ObtenerInventario();
            cmbInventario.DisplayMemberPath = "Nombre";
            cmbInventario.SelectedValuePath = "IdInventario";

            cmbFiltroCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
        }

        private void CargarProductos()
        {
            dgProductos.ItemsSource = null;
            dgProductos.ItemsSource = ProductoDAO.ObtenerTodos();
        }

        private void DgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            productoSeleccionado = dgProductos.SelectedItem as Producto;

            if (productoSeleccionado != null)
            {
                txtNombre.Text = productoSeleccionado.NombreProducto;
                txtDescripcion.Text = productoSeleccionado.DescripcionProducto;
                txtPrecio.Text = productoSeleccionado.PrecioProducto.ToString();
                txtStock.Text = productoSeleccionado.StockProducto.ToString();

                cmbCategoria.SelectedValue = productoSeleccionado.IdCategoria;
                cmbProveedor.SelectedValue = productoSeleccionado.IdProveedor;
            }
        }

        // CRUD CATEGORÍA
        private void BtnAgregarCategoria_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CategoriaDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.CategoriaEditada != null)
            {
                cmbCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
                cmbCategoria.SelectedItem = dialog.CategoriaEditada;
            }
        }

        private void BtnEditarCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategoria.SelectedItem is not Categoria categoria)
            {
                MessageBox.Show("Seleccione una categoría.");
                return;
            }

            var dialog = new CategoriaDialog(categoria);
            if (dialog.ShowDialog() == true)
            {
                cmbCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
                cmbCategoria.SelectedItem = dialog.CategoriaEditada;
            }
        }

        private void BtnEliminarCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategoria.SelectedItem is not Categoria categoria)
            {
                MessageBox.Show("Seleccione una categoría.");
                return;
            }

            var confirmar = MessageBox.Show(
                $"¿Eliminar categoría \"{categoria.Nombre}\"?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirmar == MessageBoxResult.Yes)
            {
                // Aquí debes implementar CategoriaDAO.Eliminar(id)
                if (CategoriaDAO.Eliminar(categoria.IdCategoria))
                {
                    MessageBox.Show("Categoría eliminada.");
                    cmbCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar la categoría.");
                }
            }
        }

        // CRUD PROVEEDOR
        private void BtnAgregarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProveedorDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.ProveedorEditado != null)
            {
                cmbProveedor.ItemsSource = ProveedorDAO.ObtenerTodos();
                cmbProveedor.SelectedItem = dialog.ProveedorEditado;
            }
        }

        private void BtnEditarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProveedor.SelectedItem is not Proveedor proveedor)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            var dialog = new ProveedorDialog(proveedor);
            if (dialog.ShowDialog() == true)
            {
                cmbProveedor.ItemsSource = ProveedorDAO.ObtenerTodos();
                cmbProveedor.SelectedItem = dialog.ProveedorEditado;
            }
        }

        private void BtnEliminarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProveedor.SelectedItem is not Proveedor proveedor)
            {
                MessageBox.Show("Seleccione un proveedor.");
                return;
            }

            var confirmar = MessageBox.Show(
                $"¿Eliminar proveedor \"{proveedor.Nombre}\"?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirmar == MessageBoxResult.Yes)
            {
                if (ProveedorDAO.Eliminar(proveedor.IdProveedor))
                {
                    MessageBox.Show("Proveedor eliminado.");
                    cmbProveedor.ItemsSource = ProveedorDAO.ObtenerTodos();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el proveedor.");
                }
            }
        }

        // CRUD PRODUCTO
        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtPrecio.Text, out int precio) &&
                int.TryParse(txtStock.Text, out int stock) &&
                cmbCategoria.SelectedItem is Categoria categoria &&
                cmbProveedor.SelectedItem is Proveedor proveedor)
            {
                Producto nuevo = new()
                {
                    NombreProducto = txtNombre.Text.Trim(),
                    DescripcionProducto = txtDescripcion.Text.Trim(),
                    PrecioProducto = precio,
                    StockProducto = stock,
                    IdCategoria = categoria.IdCategoria,
                    IdProveedor = proveedor.IdProveedor
                };

                if (ProductoDAO.Agregar(nuevo))
                {
                    int idProducto = ProductoDAO.ObtenerUltimoId(); // o podrías devolverlo desde el INSERT
                    InventarioDAO.Agregar(new Inventario { IdProducto = idProducto, CantidadProducto = stock });

                    MessageBox.Show("Producto agregado.");
                    LimpiarCampos();
                    CargarProductos();
                }
                else
                {
                    MessageBox.Show("Error al agregar producto.");
                }
            }
            else
            {
                MessageBox.Show("Revisa los datos ingresados.");
            }
        }

        private void BtnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto del listado.");
                return;
            }

            if (int.TryParse(txtPrecio.Text, out int precio) &&
                int.TryParse(txtStock.Text, out int stock) &&
                cmbCategoria.SelectedItem is Categoria categoria &&
                cmbProveedor.SelectedItem is Proveedor proveedor)
            {
                productoSeleccionado.NombreProducto = txtNombre.Text.Trim();
                productoSeleccionado.DescripcionProducto = txtDescripcion.Text.Trim();
                productoSeleccionado.PrecioProducto = precio;
                productoSeleccionado.StockProducto = stock;
                productoSeleccionado.IdCategoria = categoria.IdCategoria;
                productoSeleccionado.IdProveedor = proveedor.IdProveedor;

                if (ProductoDAO.Actualizar(productoSeleccionado))
                {
                    MessageBox.Show("Producto actualizado.");
                    CargarProductos();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el producto.");
                }
            }
            else
            {
                MessageBox.Show("Revisa los datos ingresados.");
            }
        }

        private void BtnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto para eliminar.");
                return;
            }

            var resultado = MessageBox.Show(
                $"¿Está seguro que desea eliminar el producto \"{productoSeleccionado.NombreProducto}\"?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado == MessageBoxResult.Yes)
            {
                if (ProductoDAO.Eliminar(productoSeleccionado.IdProducto))
                {
                    MessageBox.Show("Producto eliminado.");
                    CargarProductos();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el producto.");
                }
            }
        }

        private void BtnRefrescarProductos_Click(object sender, RoutedEventArgs e)
        {
            CargarProductos();
            txtFiltroNombre.Clear();
            cmbFiltroCategoria.SelectedIndex = -1;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            string filtroNombre = txtFiltroNombre.Text.Trim().ToLower();
            int? idCategoria = cmbFiltroCategoria.SelectedItem is Categoria cat ? cat.IdCategoria : (int?)null;

            var productos = ProductoDAO.ObtenerTodos();

            if (!string.IsNullOrWhiteSpace(filtroNombre))
                productos = [.. productos.Where(p => p.NombreProducto.Contains(filtroNombre, StringComparison.CurrentCultureIgnoreCase))];

            if (idCategoria.HasValue)
                productos = [.. productos.Where(p => p.IdCategoria == idCategoria.Value)];

            dgProductos.ItemsSource = productos;
        }

        // CRUD INVENTARIO
        private void BtnAgregarInventario_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InventarioDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.InventarioEditado != null)
            {
                cmbInventario.ItemsSource = InventarioDAO.ObtenerInventario();
                cmbInventario.SelectedItem = dialog.InventarioEditado;
            }
        }


        private void BtnEditarInventario_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInventario.SelectedItem is not Inventario inventario)
            {
                MessageBox.Show("Seleccione un inventario.");
                return;
            }

            var dialog = new InventarioDialog(inventario);
            if (dialog.ShowDialog() == true)
            {
                cmbInventario.ItemsSource = InventarioDAO.ObtenerInventario();
                cmbInventario.SelectedItem = dialog.InventarioEditado;
            }
        }

        private void BtnEliminarInventario_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInventario.SelectedItem is not Inventario inventario)
            {
                MessageBox.Show("Seleccione un inventario.");
                return;
            }

            var confirmar = MessageBox.Show(
                $"¿Eliminar inventario ID {inventario.IdInventario}?",
                "Confirmar",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (confirmar == MessageBoxResult.Yes)
            {
                if (InventarioDAO.Eliminar(inventario.IdInventario))
                {
                    MessageBox.Show("Inventario eliminado.");
                    cmbInventario.ItemsSource = InventarioDAO.ObtenerInventario();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el inventario.");
                }
            }
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            txtStock.Text = "";
            cmbCategoria.SelectedIndex = -1;
            cmbProveedor.SelectedIndex = -1;
            productoSeleccionado = null;
        }
    }
}
