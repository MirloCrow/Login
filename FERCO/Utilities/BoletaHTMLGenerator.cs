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
            string html = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Boleta #{venta.IdVenta}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; }}
        h1 {{ text-align: center; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        th, td {{ border: 1px solid #ccc; padding: 8px; text-align: center; }}
        th {{ background-color: #f4f4f4; }}
        .total {{ font-weight: bold; }}
        .footer {{ margin-top: 30px; text-align: center; font-style: italic; }}
    </style>
</head>
<body>
    <h1>FERCO - Boleta de Venta</h1>
    <p><strong>Fecha:</strong> {venta.FechaVenta}</p>
    <p><strong>ID Venta:</strong> {venta.IdVenta}</p>

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
            <td>${d.PrecioUnitario}</td>
            <td>${d.SubtotalDetalle}</td>
        </tr>";
            }

            html += $@"
        <tr class='total'>
            <td colspan='3'>TOTAL</td>
            <td>${venta.TotalVenta}</td>
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
