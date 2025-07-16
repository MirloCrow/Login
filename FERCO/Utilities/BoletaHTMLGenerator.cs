using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using FERCO.Model;

namespace FERCO.Utilities
{
    public static class BoletaHTMLGenerator
    {
        public static void GenerarBoleta(Venta venta, List<DetalleVenta> detalles)
        {
            string clienteNombre = string.IsNullOrEmpty(venta.ClienteNombre) ? "Venta Express" : venta.ClienteNombre;

            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Boleta de Compra #{venta.IdVenta}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; color: #000; }}
        h1 {{ text-align: center; margin: 0; font-size: 26px; }}
        h2 {{ text-align: center; margin: 0; font-size: 20px; color: #444; }}
        .empresa-info {{ text-align: center; margin: 5px 0 20px; font-size: 14px; }}
        .datos-venta, .datos-cliente {{ margin-top: 10px; font-size: 14px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        th, td {{ border: 1px solid #ccc; padding: 8px; text-align: center; }}
        th {{ background-color: #f9f9f9; }}
        .total {{ font-weight: bold; background-color: #f0f0f0; }}
        .footer {{ margin-top: 30px; text-align: center; font-style: italic; font-size: 13px; color: #666; }}
    </style>
</head>
<body>
    <h1>FERCO</h1>
    <h2>Boleta de Compra #{venta.IdVenta}</h2>
    <div class='empresa-info'>
        Giro: Reparación y venta de bicicletas y accesorios<br/>
        Dirección: Cruce El Molino Carretera H-30 El Molino S/N, Coltauco, Rancagua<br/>
        Teléfono: +56 9 8944 4321
    </div>

    <div class='datos-venta'>
        <strong>Fecha:</strong> {venta.FechaVenta:dd-MM-yyyy HH:mm}<br/>
        <strong>ID Venta:</strong> {venta.IdVenta}
    </div>

    <div class='datos-cliente'>
        <strong>Cliente:</strong> {clienteNombre}
    </div>

    <table>
        <tr>
            <th>Producto</th>
            <th>Cantidad</th>
            <th>Precio Unitario</th>
            <th>Subtotal</th>
        </tr>";

            foreach (var d in detalles)
            {
                int subtotal = d.SubtotalDetalle > 0
                    ? d.SubtotalDetalle
                    : d.CantidadDetalle * d.PrecioUnitario;

                html += $@"
        <tr>
            <td>{d.NombreProducto}</td>
            <td>{d.CantidadDetalle}</td>
            <td>${d.PrecioUnitario:N0}</td>
            <td>${subtotal:N0}</td>
        </tr>";
            }

            html += $@"
        <tr class='total'>
            <td colspan='3'>TOTAL</td>
            <td>${venta.TotalVenta:N0}</td>
        </tr>
    </table>

    <div class='footer'>
        ¡Gracias por su compra!<br/>
        FERCO
    </div>
</body>
</html>";

            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BoletasFERCO");
            Directory.CreateDirectory(carpeta);
            string rutaArchivo = Path.Combine(carpeta, $"Boleta_{venta.IdVenta}.html");

            File.WriteAllText(rutaArchivo, html);
            Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });
        }

        public static void GenerarBoleta(Reparacion reparacion, List<DetalleReparacion> detalles)
        {
            string clienteNombre = string.IsNullOrEmpty(reparacion.NombreCliente) ? "Cliente sin nombre" : reparacion.NombreCliente;
            string tipoReparacion = string.IsNullOrEmpty(reparacion.NombreTipoReparacion) ? "No especificado" : reparacion.NombreTipoReparacion;

            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Orden de Servicio #{reparacion.IdReparacion}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; color: #000; }}
        h1 {{ text-align: center; margin: 0; font-size: 26px; }}
        h2 {{ text-align: center; margin: 0; font-size: 20px; color: #444; }}
        .empresa-info {{ text-align: center; margin: 5px 0 20px; font-size: 14px; }}
        .datos-venta, .datos-cliente {{ margin-top: 10px; font-size: 14px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        th, td {{ border: 1px solid #ccc; padding: 8px; text-align: center; }}
        th {{ background-color: #f9f9f9; }}
        .total {{ font-weight: bold; background-color: #f0f0f0; }}
        .footer {{ margin-top: 30px; text-align: center; font-style: italic; font-size: 13px; color: #666; }}
    </style>
</head>
<body>
    <h1>FERCO</h1>
    <h2>Orden de Servicio #{reparacion.IdReparacion}</h2>
    <div class='empresa-info'>
        Giro: Reparación y venta de bicicletas y accesorios<br/>
        Dirección: Cruce El Molino Carretera H-30 El Molino S/N, Coltauco, Rancagua<br/>
        Teléfono: +56 9 8944 4321
    </div>

    <div class='datos-venta'>
        <strong>Fecha:</strong> {reparacion.FechaReparacion:dd-MM-yyyy HH:mm}<br/>
        <strong>ID Reparación:</strong> {reparacion.IdReparacion}<br/>
        <strong>Tipo de Reparación:</strong> {tipoReparacion}<br/>
        <strong>Estado:</strong> {reparacion.Estado}
    </div>

    <div class='datos-cliente'>
        <strong>Cliente:</strong> {clienteNombre}
    </div>

    <table>
        <tr>
            <th>Producto</th>
            <th>Cantidad</th>
            <th>Precio Unitario</th>
            <th>Subtotal</th>
        </tr>";

            int total = 0;

            foreach (var d in detalles)
            {
                int subtotal = d.Cantidad * d.PrecioUnitario;
                total += subtotal;

                html += $@"
        <tr>
            <td>{d.NombreProducto}</td>
            <td>{d.Cantidad}</td>
            <td>${d.PrecioUnitario:N0}</td>
            <td>${subtotal:N0}</td>
        </tr>";
            }

            html += $@"
        <tr class='total'>
            <td colspan='3'>TOTAL</td>
            <td>${total:N0}</td>
        </tr>
    </table>

    <div class='footer'>
        ¡Gracias por preferirnos!<br/>
        FERCO
    </div>
</body>
</html>";

            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BoletasFERCO");
            Directory.CreateDirectory(carpeta);
            string rutaArchivo = Path.Combine(carpeta, $"Reparacion_{reparacion.IdReparacion}.html");

            File.WriteAllText(rutaArchivo, html);
            Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });
        }
    }
}
