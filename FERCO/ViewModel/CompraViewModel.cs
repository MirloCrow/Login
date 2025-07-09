using FERCO.Data;
using FERCO.Model;
using FERCO.Utilities;
using FERCO.View.Controls;
using FERCO.View.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FERCO.ViewModel
{
    public class CompraViewModel : INotifyPropertyChanged
    {
        public Pedido PedidoActual { get; set; } = new Pedido();
        private Pedido? _pedidoSeleccionado;
        public ObservableCollection<Pedido> HistorialPedidos { get; set; } = [];
        public Pedido? PedidoSeleccionado
        {
            get => _pedidoSeleccionado;
            set
            {
                _pedidoSeleccionado = value;
                OnPropertyChanged(nameof(PedidoSeleccionado));
            }
        }
        public ObservableCollection<DetallePedido> Detalles { get; set; } = [];

        public ObservableCollection<Proveedor> Proveedores { get; set; } = [];
        
        private Proveedor? _proveedorSeleccionado;
        public Proveedor? ProveedorSeleccionado
        {
            get => _proveedorSeleccionado;
            set
            {
                _proveedorSeleccionado = value;
                PedidoActual.IdProveedor = value?.IdProveedor ?? 0;
                OnPropertyChanged(nameof(ProveedorSeleccionado));
            }
        }

        public ICommand AgregarDetalleCommand { get; set; }
        public ICommand RegistrarCompraCommand { get; set; }

        public CompraViewModel()
        {
            AgregarDetalleCommand = new RelayCommand(AgregarDetalle);
            RegistrarCompraCommand = new RelayCommand(RegistrarCompra);
            CargarProveedores();
            CargarHistorial();
        }

        private void CargarProveedores()
        {
            Proveedores.Clear();
            foreach (var p in ProveedorDAO.ObtenerTodos())
                Proveedores.Add(p);
        }
        private void CargarHistorial()
        {
            HistorialPedidos.Clear();
            foreach (var p in CompraDAO.ObtenerPedidosConDetalles())
                HistorialPedidos.Add(p);
        }

        private void AgregarDetalle()
        {
            var dialog = new AgregarDetalleDialog();
            bool? result = dialog.ShowDialog();
            if (result == true && dialog.Resultado != null)
            {
                Detalles.Add(dialog.Resultado);
            }
        }

        private void RegistrarCompra()
        {
            // 1. Cargar detalles en el modelo
            PedidoActual.Detalles = [.. Detalles];
            PedidoActual.TotalPedido = Detalles.Sum(d => d.Subtotal);

            // 2. Registrar el pedido en la BD
            var dao = new CompraDAO();
            int idPedido = CompraDAO.RegistrarPedido(PedidoActual);

            if (idPedido == 0)
            {
                MessageBox.Show("No se pudo registrar el pedido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Para cada producto, pedir ingreso a ubicaciones
            foreach (var detalle in Detalles)
            {
                var dialog = new IngresoInventarioDialog(detalle.NombreProducto ?? "Producto", detalle.Cantidad, detalle.IdProducto);
                bool? result = dialog.ShowDialog();

                if (result == true)
                {
                    foreach (var ubicacion in dialog.Resultado)
                    {
                        InventarioProductoDAO.InsertarOIncrementar(ubicacion);
                    }
                }
            }

            // Mensaje final
            MessageBox.Show("Compra registrada y stock actualizado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

            // Limpiar estado
            Detalles.Clear();
            PedidoActual = new Pedido();
            ProveedorSeleccionado = null;
            OnPropertyChanged(nameof(PedidoActual));
            CargarHistorial();
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string nombre) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombre));
    }

}
