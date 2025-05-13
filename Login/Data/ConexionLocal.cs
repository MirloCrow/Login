namespace Login.Data
{
    public static class ConexionLocal
    {
        // En Servidor se debe poner la dirección local según el dispositivo
        public static string Servidor = "R5\\SQLEXPRESS";
        public static string BaseDatos = "SistemaFERCO";

        public static string ObtenerCadenaConexion()
        {
            return $"Server={Servidor};Database={BaseDatos};Trusted_Connection=True;Encrypt=False;";
        }
    }
}
