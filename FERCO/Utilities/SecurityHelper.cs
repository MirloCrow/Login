using System;
using System.Security.Cryptography;
using System.Text;

namespace FERCO.Utilities
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Aplica un hash SHA256 a una cadena de texto, útil para almacenar contraseñas de forma segura.
        /// </summary>
        /// <param name="texto">La contraseña u otro texto plano a hashear.</param>
        /// <returns>El hash SHA256 codificado en Base64, o null si ocurre un error.</returns>
        public static string HashearSHA256(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                throw new ArgumentException("El texto a hashear no puede estar vacío o ser solo espacios.");

            byte[] bytes = Encoding.UTF8.GetBytes(texto);
            byte[] hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
