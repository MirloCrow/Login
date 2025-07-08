using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FERCO.Data;
using FERCO.Model;
using FERCO.View.Dialogs;

namespace FERCO.View
{
    public partial class ProductoControl : UserControl
    {
        private Producto? productoSeleccionado = null;
        private bool filtroStockBajoActivo = false;

        public ProductoControl()
        {
            InitializeComponent();
            CargarProductos();
        }

        private void CargarProductos()
        {
            var productos = ProductoDAO.ObtenerTodos();
            dgProductos.ItemsSource = productos;

            ActualizarAlertaStockBajo(productos);
        }

        // Stock Warning
        private void ActualizarAlertaStockBajo(List<Producto> productos)
        {
            int productosConStockBajo = productos.Count(p => p.StockTotal < 5);

            if (productosConStockBajo == 0)
            {
                txtStockBajoTitulo.Text = "Todo bien ✅";
                txtStockBajo.Text = "Productos con stock bajo: 0";
                txtStockBajo.Foreground = Brushes.DarkGreen;
                txtStockBajo.FontSize = 16;

                borderStockBajo.Background = Brushes.Honeydew;
                borderStockBajo.BorderBrush = Brushes.ForestGreen;
            }
            else
            {
                txtStockBajoTitulo.Text = "Productos con bajo stock:";
                txtStockBajo.Text = productosConStockBajo.ToString();
                txtStockBajo.Foreground = Brushes.DarkRed;
                txtStockBajo.FontSize = 20;

                borderStockBajo.Background = Brushes.Moccasin;
                borderStockBajo.BorderBrush = Brushes.DarkOrange;
            }
        }

        private void BtnStockBajoToggle_Click(object sender, RoutedEventArgs e)
        {
            if (!filtroStockBajoActivo)
            {
                // Activar filtro
                var productosBajoStock = ProductoDAO.ObtenerTodos()
                    .Where(p => p.StockTotal < 3)
                    .ToList();

                dgProductos.ItemsSource = productosBajoStock;
                filtroStockBajoActivo = true;
                btnStockBajoToggle.Content = "Mostrar todos los productos";
            }
            else
            {
                // Restaurar lista completa
                CargarProductos();
                filtroStockBajoActivo = false;
                btnStockBajoToggle.Content = "Mostrar productos con bajo stock";
            }
        }
        private void BtnExportarStockBajo_Click(object sender, RoutedEventArgs e)
        {
            var productosBajoStock = ProductoDAO.ObtenerTodos()
                .Where(p => p.StockTotal < 3)
                .ToList();

            if (productosBajoStock.Count == 0)
            {
                MessageBox.Show("No hay productos con stock bajo para exportar.");
                return;
            }

            // Aquí podrías abrir un diálogo de guardado y exportar a CSV o PDF
            MessageBox.Show("Función de exportación pendiente.");
        }

        private void SeleccionarProductoPorId(int idProducto)
        {
            var productos = ProductoDAO.ObtenerTodos();
            dgProductos.ItemsSource = productos;

            var seleccionado = productos.FirstOrDefault(p => p.IdProducto == idProducto);
            if (seleccionado != null)
            {
                dgProductos.SelectedItem = seleccionado;
                dgProductos.ScrollIntoView(seleccionado);
            }
        }

        private void DgProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProductos.SelectedItem is Producto producto)
            {
                productoSeleccionado = producto;

                txtStockTotal.Text = $"Stock total: {producto.StockTotal}";
                txtStockTotal.Foreground = producto.StockTotal == 0 ? Brushes.DarkRed : Brushes.DarkGreen;
                txtStockTotal.FontWeight = FontWeights.Bold;

                if (producto.StockTotal == 0)
                {
                    txtStockTotal.Text += " ⚠ Sin unidades disponibles";
                }

                dgUbicaciones.ItemsSource = producto.UbicacionesConStock;
            }
            else
            {
                productoSeleccionado = null;
                txtStockTotal.Text = "Stock total: -";
                dgUbicaciones.ItemsSource = null;
            }
        }

        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProductoDialog
            {
                Owner = Window.GetWindow(this)
            };
            if (dialog.ShowDialog() == true)
            {
                CargarProductos();
            }
        }

        private void BtnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (dgProductos.SelectedItem is Producto producto)
            {
                var dialog = new ProductoDialog(producto)
                {
                    Owner = Window.GetWindow(this)
                };
                if (dialog.ShowDialog() == true)
                {
                    CargarProductos();
                }
            }
            else
            {
                MessageBox.Show("Selecciona un producto para editar.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto para eliminar.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var resultado = MessageBox.Show(
                $"¿Estás seguro que deseas eliminar el producto:\n\n\"{productoSeleccionado.NombreProducto}\"?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado == MessageBoxResult.Yes)
            {
                bool eliminado = ProductoDAO.Eliminar(productoSeleccionado.IdProducto);

                if (eliminado)
                {
                    MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    productoSeleccionado = null;
                    CargarProductos();
                    dgUbicaciones.ItemsSource = null;
                    txtStockTotal.Text = "Stock total: -";
                }
                else
                {
                    MessageBox.Show("Error al eliminar el producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAgregarStock_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            // Copia segura antes del diálogo
            var producto = productoSeleccionado;

            var dialog = new StockDialog(producto.IdProducto)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                CargarProductos();
                SeleccionarProductoPorId(producto.IdProducto); // reutilizamos la lógica segura
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

        private void BtnBuscar_Click(object sender, RoutedEventArgs e)
        {
            string filtro = txtBuscar.Text.Trim().ToLower();
            string tipoBusqueda = ((ComboBoxItem)cmbTipoBusqueda.SelectedItem).Content.ToString() ?? "";

            var productos = ProductoDAO.ObtenerTodos();

            switch (tipoBusqueda)
            {
                case "Nombre":
                    productos = [.. productos.Where(p => p.NombreProducto?.ToLower().Contains(filtro, StringComparison.CurrentCultureIgnoreCase) == true)];
                    break;
                case "Código":
                    productos = [.. productos.Where(p => p.CodigoProducto?.ToLower().Contains(filtro, StringComparison.CurrentCultureIgnoreCase) == true)];
                    break;
                case "Categoría":
                    productos = [.. productos.Where(p => p.NombreCategoria?.ToLower().Contains(filtro, StringComparison.CurrentCultureIgnoreCase) == true)];
                    break;
            }

            if (productos.Count == 0)
            {
                MessageBox.Show("No se encontraron productos con ese criterio de búsqueda.", "Sin resultados", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            dgProductos.ItemsSource = productos;
        }

        private void TxtBuscar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnBuscar_Click(sender, e);
            }
        }
    }
}
