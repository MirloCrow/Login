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
    <title>Boleta #{venta.IdVenta}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; color: #000; }}
        h1, h2 {{ text-align: center; margin: 0; }}
        .empresa-info {{ text-align: center; margin-top: 5px; margin-bottom: 20px; font-size: 14px; }}
        .datos-venta, .datos-cliente {{ margin-top: 10px; font-size: 14px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        th, td {{ border: 1px solid #ccc; padding: 8px; text-align: center; }}
        th {{ background-color: #f4f4f4; }}
        .total {{ font-weight: bold; }}
        .footer {{ margin-top: 30px; text-align: center; font-style: italic; font-size: 14px; }}
    </style>
</head>
<body>
    <h1>FERCO</h1>
    <div class='empresa-info'>
        Giro: Reparación y venta de bicicletas y accesorios.<br/>
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
                html += $@"
        <tr>
            <td>{d.ProductoNombre}</td>
            <td>{d.CantidadDetalle}</td>
            <td>${d.PrecioUnitario:N0}</td>
            <td>${d.SubtotalDetalle:N0}</td>
        </tr>";
            }

            html += $@"
        <tr class='total'>
            <td colspan='3'>TOTAL</td>
            <td>${venta.TotalVenta:N0}</td>
        </tr>
    </table>

    <div class='footer'>
        ¡Gracias por su compra!
    </div>
</body>
</html>";

            // Guardar archivo
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BoletasFERCO");
            Directory.CreateDirectory(carpeta);
            string rutaArchivo = Path.Combine(carpeta, $"Boleta_{venta.IdVenta}.html");

            File.WriteAllText(rutaArchivo, html);

            // Abrir en navegador
            Process.Start(new ProcessStartInfo(rutaArchivo) { UseShellExecute = true });
        }

    }
}
