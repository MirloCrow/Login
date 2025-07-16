using FERCO.Model;
using Microsoft.Data.SqlClient;
using FERCO.Utilities;

namespace FERCO.Data
{
    public static class UsuarioDAO
    {
        public static bool ExisteAlMenosUnUsuario()
        {
            try
            {
                using SqlConnection conn = ConexionBD.ObtenerConexion();
                string query = "SELECT COUNT(*) FROM Usuario";
                using SqlCommand cmd = new(query, conn);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[UsuarioDAO] Error al verificar existencia de usuarios: " + ex.Message);
                return false; // En caso de error, asumir que no hay usuarios para prevenir fallas graves
            }
        }

        public static bool AgregarUsuario(string nombre, string contrasenaPlano, string rol)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(contrasenaPlano) || string.IsNullOrWhiteSpace(rol))
                    throw new ArgumentException("Los parámetros no pueden estar vacíos.");

                string hash = SecurityHelper.HashearSHA256(contrasenaPlano) ?? throw new InvalidOperationException("No se pudo generar el hash de la contraseña.");
                using SqlConnection conn = ConexionBD.ObtenerConexion();
                string query = "INSERT INTO Usuario (nombre_usuario, contrasena, rol) VALUES (@nombre, @hash, @rol)";
                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@nombre", nombre);
                cmd.Parameters.AddWithValue("@hash", hash);
                cmd.Parameters.AddWithValue("@rol", rol);
                conn.Open();
                cmd.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[UsuarioDAO] Error al agregar usuario: " + ex.Message);
                return false;
            }
        }

        public static Usuario? ObtenerPorCredenciales(string nombreUsuario, string contrasenaHasheada)
        {
            try
            {
                using SqlConnection conn = ConexionBD.ObtenerConexion();
                conn.Open();

                string query = @"
            SELECT nombre_usuario, contrasena, rol 
            FROM Usuario 
            WHERE nombre_usuario = @usuario AND contrasena = @hash";

                using SqlCommand cmd = new(query, conn);
                cmd.Parameters.AddWithValue("@usuario", nombreUsuario);
                cmd.Parameters.AddWithValue("@hash", contrasenaHasheada);

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new Usuario
                    {
                        NombreUsuario = reader["nombre_usuario"].ToString() ?? "",
                        Contrasena = "", // Por seguridad, no se devuelve la contraseña
                        Rol = reader["rol"].ToString() ?? ""
                    };
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UsuarioDAO] Error en ObtenerPorCredenciales: " + ex.Message);
                return null;
            }
        }
    }
}