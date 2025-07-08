using FERCO.Model;
using System.IO;
using System.Text.Json;

namespace FERCO.Utilities
{
    public static class ConfiguracionManager
    {
        private static readonly string configPath = "config.json";
        public static ConfiguracionApp Config { get; private set; } = null!;

        static ConfiguracionManager()
        {
            CargarConfiguracion();
        }

        public static void CargarConfiguracion()
        {
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                Config = JsonSerializer.Deserialize<ConfiguracionApp>(json) ?? new ConfiguracionApp();
            }
            else
            {
                Config = new ConfiguracionApp(); // valores por defecto
                GuardarConfiguracion();
            }
        }

        public static void GuardarConfiguracion()
        {
            var json = JsonSerializer.Serialize(Config, opcionesJson);
            File.WriteAllText(configPath, json);
        }

        private static readonly JsonSerializerOptions opcionesJson = new()
        {
            WriteIndented = true
        };
    }
}