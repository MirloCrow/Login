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
using FERCO.View.Dialogs;

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

            cmbInventario.ItemsSource = InventarioDAO.ObtenerInventarios();
            cmbInventario.DisplayMemberPath = "Descripcion";
            cmbInventario.SelectedValuePath = "IdInventario";
        }

        private void CargarProductos()
        {
            dgProductos.ItemsSource = null;
            dgProductos.ItemsSource = ProductoDAO.ObtenerTodos();
        }

        private void DgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var seleccionado = dgProductos.SelectedItem as Producto;
            productoSeleccionado = seleccionado;

            if (seleccionado != null)
            {
                txtNombre.Text = seleccionado.NombreProducto;
                txtDescripcion.Text = seleccionado.DescripcionProducto;
                txtPrecio.Text = seleccionado.PrecioProducto.ToString();

                cmbCategoria.SelectedValue = seleccionado.IdCategoria;
                cmbProveedor.SelectedValue = seleccionado.IdProveedor;

                dgUbicaciones.ItemsSource = seleccionado.UbicacionesConStock;

                // Mostrar stock total con advertencia
                int stockTotal = seleccionado.StockTotal;
                txtStockTotal.Text = $"Stock total: {stockTotal}";

                if (stockTotal == 0)
                {
                    txtStockTotal.Foreground = Brushes.DarkRed;
                    txtStockTotal.FontWeight = FontWeights.Bold;
                    txtStockTotal.Text += " ⚠ Sin unidades disponibles";
                }
                else
                {
                    txtStockTotal.Foreground = Brushes.DarkGreen;
                    txtStockTotal.FontWeight = FontWeights.Bold;
                }

                // Seleccionar automáticamente la ubicación con más stock
                var inventarioMayorStock = seleccionado.UbicacionesConStock?
                    .OrderByDescending(u => u.Cantidad)
                    .FirstOrDefault();

                if (inventarioMayorStock != null)
                {
                    cmbInventario.SelectedValue = inventarioMayorStock.IdInventario;
                }
                else
                {
                    cmbInventario.SelectedIndex = -1;
                }

                // Mostrar el panel de agregar stock
                panelAgregarStock.Visibility = Visibility.Visible;
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
                cmbCategoria.SelectedItem is Categoria categoria &&
                cmbProveedor.SelectedItem is Proveedor proveedor)
            {
                string nombre = txtNombre.Text.Trim();
                
                // Verificar si ya existe un producto con ese nombre
                Producto? existente = ProductoDAO.BuscarPorNombre(nombre);

                if (existente != null)
                {
                    MessageBox.Show("Ya existe un producto con ese nombre.");
                    return;
                }

                // Crear nuevo producto
                Producto nuevo = new()
                {
                    NombreProducto = nombre,
                    DescripcionProducto = txtDescripcion.Text.Trim(),
                    CodigoProducto = txtCodigo.Text.Trim(),
                    PrecioProducto = precio,
                    IdCategoria = categoria.IdCategoria,
                    IdProveedor = proveedor.IdProveedor
                };

                if (ProductoDAO.Agregar(nuevo))
                {
                    MessageBox.Show("Producto agregado. Ahora puedes registrar su stock.");
                    CargarProductos();
                    LimpiarCampos();
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
                cmbCategoria.SelectedItem is Categoria categoria &&
                cmbProveedor.SelectedItem is Proveedor proveedor)
            {
                productoSeleccionado.NombreProducto = txtNombre.Text.Trim();
                productoSeleccionado.DescripcionProducto = txtDescripcion.Text.Trim();
                productoSeleccionado.PrecioProducto = precio;
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

        // CRUD INVENTARIO
        private void BtnAgregarInventario_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InventarioDialog
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true && dialog.InventarioEditado != null)
            {
                cmbInventario.ItemsSource = InventarioDAO.ObtenerInventarios();
                cmbInventario.SelectedValue = dialog.InventarioEditado.IdInventario;
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
                cmbInventario.ItemsSource = InventarioDAO.ObtenerInventarios();
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
                    cmbInventario.ItemsSource = InventarioDAO.ObtenerInventarios();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el inventario.");
                }
            }
        }
        private void DgUbicaciones_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit &&
                e.Row.Item is InventarioProducto item &&
                e.Column.Header.ToString() == "Stock")
            {
                if (e.EditingElement is TextBox tb &&
                    int.TryParse(tb.Text, out int nuevaCantidad) &&
                    nuevaCantidad >= 0)
                {
                    item.Cantidad = nuevaCantidad;
                    bool ok = InventarioProductoDAO.Actualizar(item);
                    if (!ok)
                        MessageBox.Show("Error al actualizar el stock en la base de datos.");
                }
                else
                {
                    MessageBox.Show("Cantidad inválida.");
                    e.Cancel = true;
                }
            }
        }
        private void AgregarStockUbicacion(int idProducto, int cantidad, int idInventario)
        {
            var inventarioProducto = new InventarioProducto
            {
                IdProducto = idProducto,
                IdInventario = idInventario,
                Cantidad = cantidad
            };

            var ubicaciones = InventarioProductoDAO.ObtenerUbicacionesPorProducto(idProducto);
            bool yaExiste = ubicaciones.Any(ip => ip.IdInventario == idInventario);

            bool ok = yaExiste
                ? InventarioProductoDAO.Actualizar(inventarioProducto)
                : InventarioProductoDAO.Insertar(inventarioProducto);

            if (ok)
                MessageBox.Show("Stock actualizado.");
            else
                MessageBox.Show("Error al actualizar el stock.");

            LimpiarCampos();
            CargarProductos();
        }
        private void BtnAgregarStock_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            if (!int.TryParse(txtStockNuevo.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.");
                return;
            }

            if (cmbInventario.SelectedItem is not Inventario inventario)
            {
                MessageBox.Show("Seleccione una ubicación de inventario.");
                return;
            }

            var ubicaciones = InventarioProductoDAO.ObtenerUbicacionesPorProducto(productoSeleccionado.IdProducto);
            var existente = ubicaciones.FirstOrDefault(u => u.IdInventario == inventario.IdInventario);

            int nuevaCantidad = existente != null
                ? existente.Cantidad + cantidad
                : cantidad;

            var nuevoRegistro = new InventarioProducto
            {
                IdProducto = productoSeleccionado.IdProducto,
                IdInventario = inventario.IdInventario,
                Cantidad = nuevaCantidad
            };

            bool ok = existente != null
                ? InventarioProductoDAO.Actualizar(nuevoRegistro)
                : InventarioProductoDAO.Insertar(nuevoRegistro);

            if (ok)
            {
                MessageBox.Show("Stock actualizado.");

                int id = productoSeleccionado.IdProducto;
                CargarProductos();
                SeleccionarProductoPorId(id);

                txtStockNuevo.Text = "";
            }
            else
            {
                MessageBox.Show("Error al actualizar stock.");
            }
        }

        private void SeleccionarProductoPorId(int idProducto)
        {
            var productos = ProductoDAO.ObtenerTodos();
            dgProductos.ItemsSource = productos;

            var seleccionado = productos.FirstOrDefault(p => p.IdProducto == idProducto);
            if (seleccionado != null)
                dgProductos.SelectedItem = seleccionado;
        }



        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();
            string tipoBusqueda = ((ComboBoxItem)cmbTipoBusqueda.SelectedItem).Content.ToString() ?? "";

            var productos = ProductoDAO.ObtenerTodos();

            switch (tipoBusqueda)
            {
                case "Nombre":
                    productos = [.. productos.Where(p => p.NombreProducto?.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)];

                    break;

                case "Código":
                    // No implementado aún → mostrar todos
                    break;

                case "Categoría":
                    productos = [.. productos.Where(p => p.NombreCategoria?.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)];

                    break;
            }

            dgProductos.ItemsSource = productos;
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            txtStockTotal.Text = "Stock total: -";
            txtStockTotal.Foreground = Brushes.Black;
            txtStockTotal.FontWeight = FontWeights.Normal;
            txtStockNuevo.Text = "";
            txtBuscar.Text = "";
            cmbCategoria.SelectedIndex = -1;
            cmbProveedor.SelectedIndex = -1;
            cmbInventario.SelectedIndex = -1;
            dgUbicaciones.ItemsSource = null;
            dgProductos.SelectedItem = null;
            productoSeleccionado = null;
            panelAgregarStock.Visibility = Visibility.Collapsed;
            CargarProductos();
        }
    }
}
