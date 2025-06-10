using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

        private void CargarCombos()
        {
            cmbProveedor.ItemsSource = ProveedorDAO.ObtenerTodos();
            cmbProveedor.DisplayMemberPath = "Nombre";
            cmbProveedor.SelectedValuePath = "IdProveedor";

            cmbCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
            cmbCategoria.DisplayMemberPath = "Nombre";
            cmbCategoria.SelectedValuePath = "IdCategoria";

            cmbInventario.ItemsSource = InventarioDAO.ObtenerUbicacionesConProductos();
            cmbInventario.DisplayMemberPath = "Descripcion";
            cmbInventario.SelectedValuePath = "IdInventario";

            cmbFiltroCategoria.ItemsSource = CategoriaDAO.ObtenerTodas();
        }

        private void CargarProductos()
        {
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

                cmbCategoria.SelectedValue = productoSeleccionado.IdCategoria;
                cmbProveedor.SelectedValue = productoSeleccionado.IdProveedor;
            }
        }

        private void BtnAgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarDatosProducto(out int precio, out int stock, out Categoria categoria, out Proveedor proveedor, out UbicacionInventario ubicacion)) return;

            Producto nuevo = new()
            {
                NombreProducto = txtNombre.Text.Trim(),
                DescripcionProducto = txtDescripcion.Text.Trim(),
                PrecioProducto = precio,
                IdCategoria = categoria.IdCategoria,
                IdProveedor = proveedor.IdProveedor
            };

            if (ProductoDAO.Agregar(nuevo))
            {
                int idProducto = ProductoDAO.ObtenerUltimoId();

                if (!InventarioProductoDAO.Agregar(new InventarioProducto
                {
                    IdInventario = ubicacion.IdInventario,
                    IdProducto = idProducto,
                    Cantidad = stock
                }))
                {
                    MessageBox.Show("Error al asignar inventario al producto.");
                }

                MessageBox.Show("Producto agregado correctamente.");
                LimpiarCampos();
                CargarProductos();
            }
            else
            {
                MessageBox.Show("Error al agregar el producto.");
            }
        }

        private void BtnEditarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto del listado.");
                return;
            }

            if (!ValidarDatosProducto(out int precio, out int stock, out Categoria categoria, out Proveedor proveedor, out UbicacionInventario ubicacion)) return;

            productoSeleccionado.NombreProducto = txtNombre.Text.Trim();
            productoSeleccionado.DescripcionProducto = txtDescripcion.Text.Trim();
            productoSeleccionado.PrecioProducto = precio;
            productoSeleccionado.IdCategoria = categoria.IdCategoria;
            productoSeleccionado.IdProveedor = proveedor.IdProveedor;

            var productosEnUbicacion = InventarioProductoDAO.ObtenerProductosPorUbicacion(ubicacion.IdInventario);
            var existente = productosEnUbicacion.FirstOrDefault(ip => ip.IdProducto == productoSeleccionado.IdProducto);

            if (existente != null)
            {
                existente.Cantidad = stock;
                InventarioProductoDAO.Actualizar(existente);
            }
            else
            {
                InventarioProductoDAO.Agregar(new InventarioProducto
                {
                    IdInventario = ubicacion.IdInventario,
                    IdProducto = productoSeleccionado.IdProducto,
                    Cantidad = stock
                });
            }

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

        private bool ValidarDatosProducto(out int precio, out int stock, out Categoria categoria, out Proveedor proveedor, out UbicacionInventario ubicacion)
        {
            precio = 0; stock = 0; categoria = null!; proveedor = null!; ubicacion = null!;

            if (string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtDescripcion.Text) ||
                !int.TryParse(txtPrecio.Text, out precio) ||
                !int.TryParse(txtStock.Text, out stock) ||
                cmbCategoria.SelectedItem is not Categoria cat ||
                cmbProveedor.SelectedItem is not Proveedor prov ||
                cmbInventario.SelectedItem is not UbicacionInventario inv)
            {
                MessageBox.Show("Revisa los datos ingresados.");
                return false;
            }

            categoria = cat;
            proveedor = prov;
            ubicacion = inv;
            return true;
        }

        private void BtnAgregarCategoria_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CategoriaDialog();
            if (dialog.ShowDialog() == true)
            {
                CargarCombos(); // Refresca el ComboBox
                cmbCategoria.SelectedValue = dialog.CategoriaEditada.IdCategoria;
            }
        }

        private void BtnEditarCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategoria.SelectedItem is Categoria categoria)
            {
                var dialog = new CategoriaDialog(categoria);
                if (dialog.ShowDialog() == true)
                {
                    CargarCombos();
                    cmbCategoria.SelectedValue = dialog.CategoriaEditada.IdCategoria;
                }
            }
        }

        private void BtnEliminarCategoria_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategoria.SelectedItem is Categoria categoria)
            {
                if (MessageBox.Show($"¿Seguro que deseas eliminar la categoría '{categoria.Nombre}'?",
                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (CategoriaDAO.Eliminar(categoria.IdCategoria))
                    {
                        CargarCombos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar la categoría.");
                    }
                }
            }
        }

        private void BtnAgregarProveedor_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ProveedorDialog();
            if (dialog.ShowDialog() == true)
            {
                CargarCombos();
                cmbProveedor.SelectedValue = dialog.ProveedorEditado.IdProveedor;
            }
        }

        private void BtnEditarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProveedor.SelectedItem is Proveedor proveedor)
            {
                var dialog = new ProveedorDialog(proveedor);
                if (dialog.ShowDialog() == true)
                {
                    CargarCombos();
                    cmbProveedor.SelectedValue = dialog.ProveedorEditado.IdProveedor;
                }
            }
        }

        private void BtnEliminarProveedor_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProveedor.SelectedItem is Proveedor proveedor)
            {
                if (MessageBox.Show($"¿Seguro que deseas eliminar al proveedor '{proveedor.Nombre}'?",
                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (ProveedorDAO.Eliminar(proveedor.IdProveedor))
                    {
                        CargarCombos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el proveedor.");
                    }
                }
            }
        }

        private void BtnAgregarInventario_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InventarioDialog();
            if (dialog.ShowDialog() == true)
            {
                CargarCombos();
                cmbInventario.SelectedValue = dialog.InventarioEditado.IdInventario;
            }
        }

        private void BtnEditarInventario_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInventario.SelectedItem is UbicacionInventario inv)
            {
                var dialog = new InventarioDialog(inv);
                if (dialog.ShowDialog() == true)
                {
                    CargarCombos();
                    cmbInventario.SelectedValue = dialog.InventarioEditado.IdInventario;
                }
            }
        }

        private void BtnEliminarInventario_Click(object sender, RoutedEventArgs e)
        {
            if (cmbInventario.SelectedItem is UbicacionInventario inv)
            {
                if (MessageBox.Show($"¿Seguro que deseas eliminar la ubicación '{inv.Descripcion}'?",
                    "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (InventarioDAO.EliminarUbicacion(inv.IdInventario))
                    {
                        CargarCombos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar la ubicación.");
                    }
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
            cmbInventario.SelectedIndex = -1;

            productoSeleccionado = null;
            dgProductos.UnselectAll();
        }
        private void BtnEliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            if (productoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un producto a eliminar.");
                return;
            }

            var ubicacionesConStock = ObtenerUbicacionesConStock(productoSeleccionado.IdProducto);

            if (ubicacionesConStock.Any())
            {
                string mensaje = "No se puede eliminar el producto porque tiene stock en las siguientes ubicaciones:\n\n";
                foreach (var (ubicacion, cantidad) in ubicacionesConStock)
                {
                    mensaje += $"- {ubicacion}: {cantidad} unidades\n";
                }
                MessageBox.Show(mensaje, "Producto con stock asignado", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show($"¿Seguro que deseas eliminar el producto '{productoSeleccionado.NombreProducto}'?",
                "Confirmar eliminación", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                InventarioProductoDAO.EliminarPorProducto(productoSeleccionado.IdProducto);

                if (ProductoDAO.Eliminar(productoSeleccionado.IdProducto))
                {
                    MessageBox.Show("Producto eliminado correctamente.");
                    LimpiarCampos();
                    CargarProductos();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el producto.");
                }
            }
        }


        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            string nombreFiltro = txtFiltroNombre.Text.Trim().ToLower();
            int? idCategoria = cmbFiltroCategoria.SelectedValue as int?;

            var productos = ProductoDAO.ObtenerTodos();

            var filtrados = productos.Where(p =>
                (string.IsNullOrEmpty(nombreFiltro) || p.NombreProducto.ToLower().Contains(nombreFiltro)) &&
                (!idCategoria.HasValue || p.IdCategoria == idCategoria.Value)
            ).ToList();

            dgProductos.ItemsSource = filtrados;
        }
        private void BtnRefrescarProductos_Click(object sender, RoutedEventArgs e)
        {
            txtFiltroNombre.Text = "";
            cmbFiltroCategoria.SelectedIndex = -1;
            CargarProductos();
        }
        private bool ProductoTieneStockAsignado(int idProducto)
        {
            var ubicaciones = InventarioDAO.ObtenerUbicacionesConProductos();

            foreach (var ubicacion in ubicaciones)
            {
                var productos = InventarioProductoDAO.ObtenerProductosPorUbicacion(ubicacion.IdInventario);
                var encontrado = productos.FirstOrDefault(p => p.IdProducto == idProducto);

                if (encontrado != null && encontrado.Cantidad > 0)
                {
                    return true;
                }
            }

            return false;
        }
        private List<(string ubicacion, int cantidad)> ObtenerUbicacionesConStock(int idProducto)
        {
            var ubicacionesConStock = new List<(string, int)>();

            var ubicaciones = InventarioDAO.ObtenerUbicacionesConProductos();

            foreach (var ubicacion in ubicaciones)
            {
                var productos = InventarioProductoDAO.ObtenerProductosPorUbicacion(ubicacion.IdInventario);
                var encontrado = productos.FirstOrDefault(p => p.IdProducto == idProducto);

                if (encontrado != null && encontrado.Cantidad > 0)
                {
                    ubicacionesConStock.Add((ubicacion.Descripcion, encontrado.Cantidad));
                }
            }

            return ubicacionesConStock;
        }


    }
}
