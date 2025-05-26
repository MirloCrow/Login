
# 📦 Proyecto SistemaFERCO - Conexión Local

Este proyecto usa SQL Server. Está preparado para que cada desarrollador pueda usar su propia configuración local de base de datos.

---

## ✅ ¿Cómo configurar tu entorno?

1. Abre el archivo:

```
Data/ConexionLocal.cs
```

2. Cambia las siguientes líneas con tus datos reales:

```csharp
public static string Servidor = "TU_SERVIDOR\SQLEXPRESS";
public static string BaseDatos = "SistemaFERCO";
```

Ejemplo:

```csharp
public static string Servidor = "DESKTOP123\SQLEXPRESS";
public static string BaseDatos = "SistemaFERCO_TEST";
```

---

## 🛡 Archivo excluido del repositorio

El archivo `ConexionLocal.cs` está **ignorado por Git** (gracias a `.gitignore`), por lo que:

- No se sube al repositorio
- Cada desarrollador puede usar su propia configuración

Si no lo tienes, copia este contenido en `Data/ConexionLocal.cs`:

```csharp
namespace FERCO.Data
{
    public static class ConexionLocal
    {
        public static string Servidor = "TU_SERVIDOR\SQLEXPRESS";
        public static string BaseDatos = "SistemaFERCO";

        public static string ObtenerCadenaConexion()
        {
            return $"Server={Servidor};Database={BaseDatos};Trusted_Connection=True;Encrypt=False;";
        }
    }
}
```

---

## 🚀 ¡Listo para trabajar!

Con tu `ConexionLocal.cs` configurado, puedes ejecutar la app sin tocar el resto del proyecto.
