# FERCO - Sistema de Gestión para Taller de Bicicletas

FERCO es una aplicación de escritorio WPF desarrollada para la gestión integral de un taller de bicicletas. 
Permite administrar productos, inventario, ventas, reparaciones y clientes, utilizando una arquitectura MVVM robusta y escalable.

---

## 🧩 Módulo de Reparaciones

Este módulo incluye:

- Registro de reparaciones para clientes.
- Selección de tipos de reparación predefinidos.
- Asignación automática de productos necesarios según el tipo de reparación.
- Control del estado de reparación: `Pendiente`, `En proceso`, `Completada`.
- Visualización y filtrado de reparaciones ya registradas.
- Actualización de stock automático al agendar.

---

## 🆕 Versión de base de datos usada: `test3`

En esta versión se incorporan las siguientes nuevas tablas para gestionar tipos de reparaciones predefinidas y los productos requeridos por cada tipo:

```sql
-- Tabla de tipos de reparación
CREATE TABLE TipoReparacion (
  id_tipo_reparacion INT IDENTITY(1,1) PRIMARY KEY,
  nombre NVARCHAR(255) NOT NULL,
  descripcion NVARCHAR(255)
);

-- Asociación entre tipo de reparación y productos requeridos
CREATE TABLE TipoReparacionProducto (
  id_tipo_reparacion INT NOT NULL,
  id_producto INT NOT NULL,
  cantidad_requerida INT NOT NULL,
  PRIMARY KEY (id_tipo_reparacion, id_producto),
  FOREIGN KEY (id_tipo_reparacion) REFERENCES TipoReparacion(id_tipo_reparacion),
  FOREIGN KEY (id_producto) REFERENCES Producto(id_producto)
);
```

Estas tablas permiten registrar configuraciones de reparación reutilizables que facilitan la selección y la gestión de productos asociados.

---

## ✅ Funcionalidades adicionales planificadas

| Funcionalidad            | Descripción |
|--------------------------|-------------|
| 🧾 Generar orden          | Crear comprobante u orden de trabajo imprimible o en PDF |
| 📝 Observaciones          | Campo para agregar detalles adicionales por reparación |
| 📅 Historial              | Ver reparaciones anteriores del cliente |
| 🧩 Reparaciones múltiples | Crear múltiples reparaciones sin cerrar la ventana |
| 🧑‍🔧 Técnico asignado     | Asignar responsable de la reparación (opcional) |

---

## 📁 Estructura del Proyecto

```
FERCO/
├── View/
│   ├── Controls/
│   │   ├── ReparacionControl.xaml
│   │   ├── SeguimientoReparacionesControl.xaml
│   └── Dialogs/
├── ViewModel/
│   ├── ReparacionViewModel.cs
│   └── SeguimientoReparacionesViewModel.cs
├── Model/
│   ├── Reparacion.cs
│   └── ReparacionEditable.cs
├── Data/
│   └── ReparacionDAO.cs
├── Utilities/
│   └── RelayCommand.cs
└── App.xaml
```

---

## 🛠 Requisitos

- .NET 6 o superior
- Visual Studio 2022+
- SQL Server Express o LocalDB

---

## 🚀 Instrucciones para ejecutar

1. Clona el repositorio.
2. Abre la solución `FERCO.sln` en Visual Studio.
3. Asegúrate de tener configurada la conexión a la base de datos `test3` en `ConexionLocal.cs`.
4. Ejecuta el proyecto.


