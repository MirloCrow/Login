using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.SqlClient;
using Login.Data;

namespace LoginApp
{
    public partial class VentanaVentas : Window
    {
        public VentanaVentas()
        {
            InitializeComponent();
            CargarProductos();
            CargarStock();
        }

        private void CargarProductos()
        {
            try
            {
                using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                {
                    string query = "SELECT id_producto, nombre_producto FROM Producto";
                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        cmbProducto.Items.Add(new
                        {
                            Id = lector["id_producto"],
                            Nombre = lector["nombre_producto"].ToString()
                        });
                    }

                    cmbProducto.DisplayMemberPath = "Nombre";
                    cmbProducto.SelectedValuePath = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private void BtnRegistrarVenta_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProducto.SelectedItem == null || string.IsNullOrWhiteSpace(txtCantidad.Text))
            {
                lblMensaje.Text = "Seleccione un producto y cantidad válida.";
                return;
            }

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                lblMensaje.Text = "Cantidad no válida.";
                return;
            }

            int idProducto = (int)cmbProducto.SelectedValue;

            try
            {
                using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                {
                    // Primero, verificar stock actual
                    string consultaStock = "SELECT stock_producto FROM Producto WHERE id_producto = @id";
                    SqlCommand cmdVerificar = new SqlCommand(consultaStock, conexion);
                    cmdVerificar.Parameters.AddWithValue("@id", idProducto);
                    int stockActual = Convert.ToInt32(cmdVerificar.ExecuteScalar());

                    if (cantidad > stockActual)
                    {
                        lblMensaje.Text = $"Stock insuficiente. Stock disponible: {stockActual}";
                        return;
                    }

                    // Restar stock
                    string updateStock = "UPDATE Producto SET stock_producto = stock_producto - @cantidad WHERE id_producto = @id";
                    SqlCommand cmdStock = new SqlCommand(updateStock, conexion);
                    cmdStock.Parameters.AddWithValue("@cantidad", cantidad);
                    cmdStock.Parameters.AddWithValue("@id", idProducto);
                    int filasAfectadas = cmdStock.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        lstVentas.Items.Add($"Venta: Producto #{idProducto}, Cantidad: {cantidad}");
                        lblMensaje.Text = "Venta registrada exitosamente.";
                        CargarStock(); // actualiza la lista de stock
                        txtCantidad.Clear();
                        cmbProducto.SelectedIndex = -1;
                    }
                    else
                    {
                        lblMensaje.Text = "No se pudo registrar la venta.";
                    }
                }
            }
            catch (Exception ex)
            {
                lblMensaje.Text = "Error: " + ex.Message;
            }
        }

        private void CargarStock()
        {
            lstStock.Items.Clear();

            try
            {
                using (SqlConnection conexion = ConexionBD.ObtenerConexion())
                {
                    string query = "SELECT nombre_producto, stock_producto FROM Producto";
                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataReader lector = comando.ExecuteReader();

                    while (lector.Read())
                    {
                        lstStock.Items.Add($"{lector["nombre_producto"]}: {lector["stock_producto"]} unidades");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar stock: " + ex.Message);
            }
        }
    }
}
