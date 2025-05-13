using Microsoft.Data.SqlClient;
using Login.Model;

namespace Login.Data
{
    public class UsuarioDAO
    {
        public static Usuario ValidarLogin(string nombre, string contrasena)
        {
            using var con = ConexionBD.ObtenerConexion();
            var cmd = new SqlCommand("SELECT * FROM Usuario WHERE nombre_usuario = @nombre AND contrasena = @clave", con);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            cmd.Parameters.AddWithValue("@clave", contrasena);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    IdUsuario = (int)reader["id_usuario"],
                    NombreUsuario = reader["nombre_usuario"].ToString(),
                    Contrasena = reader["contrasena"].ToString(),
                    Rol = reader["rol"].ToString()
                };
            }
            return null;
        }
    }
}

