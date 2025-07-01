namespace FERCO.Data
{
    public static class ConexionLocal
    {
        // En Servidor se debe poner la dirección local según el dispositivo
        public static string Servidor = "LAPTOP-HK8AMDPP\\GESTION_FERCO";
        public static string BaseDatos = "test19";

        public static string ObtenerCadenaConexion()
        {
            return $"Server={Servidor};Database={BaseDatos};Trusted_Connection=True;Encrypt=False;";
        }
    }
}
