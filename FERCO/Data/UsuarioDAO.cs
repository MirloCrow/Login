using FERCO.Model;
using Microsoft.Data.SqlClient;

namespace FERCO.Data
{
    public static class UsuarioDAO
    {
        public static Usuario? ObtenerPorCredenciales(string usuario, string contrasena)
        {
            using SqlConnection conn = ConexionBD.ObtenerConexion();
            conn.Open();

            string query = "SELECT nombre_usuario, contrasena, rol FROM Usuario WHERE nombre_usuario = @usuario AND contrasena = @contrasena";

            using SqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@usuario", usuario);
            cmd.Parameters.AddWithValue("@contrasena", contrasena);

            using SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    NombreUsuario = reader["nombre_usuario"].ToString() ?? "",
                    Contrasena = reader["contrasena"].ToString() ?? "",
                    Rol = reader["rol"].ToString() ?? ""
                };
            }

            return null;
        }
    }
}