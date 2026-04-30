# 🚗 SITAUTO - Sistema de Gestión para Taller Automotriz

SITAUTO es una aplicación de escritorio desarrollada en C# con WPF y SQL Server, diseñada para ayudar a talleres automotrices a gestionar eficientemente sus clientes, vehículos, órdenes de servicio, empleados y más.

---

## 📌 Objetivos del Proyecto

### 🎯 Objetivo General
Desarrollar un sistema de gestión integral para talleres automotrices que facilite la administración de clientes, vehículos, servicios, empleados y reportes.

### ✅ Objetivos Específicos
- Crear una base de datos relacional robusta.
- Permitir el registro, edición y eliminación de clientes, vehículos y empleados.
- Gestionar órdenes de servicio con estado y seguimiento.
- Ofrecer estadísticas e informes automatizados.
- Brindar control de acceso según tipo de usuario.

---

## 📐 Alcance del Proyecto

- Registro y gestión de clientes, vehículos y empleados.
- Generación y administración de órdenes de servicio.
- Visualización de estadísticas (total de clientes, vehículos y órdenes activas).
- Módulo de facturación (básico).
- Módulo de agenda y reportes.
- Control de acceso a funcionalidades por rol de usuario (admin/usuario).

---

## 💡 Justificación y Relevancia

El sistema SITAUTO responde a la necesidad de los talleres mecánicos de digitalizar sus operaciones para mejorar la eficiencia, reducir errores y ofrecer un mejor servicio al cliente. El sistema es útil, escalable y fácil de utilizar.

---

## ⚙️ Requerimientos

### 🔧 Requerimientos Funcionales
- Registro y edición de clientes, empleados y vehículos.
- Gestión de órdenes de trabajo por cliente y vehículo.
- Reportes de actividad y estadísticas.
- Control de acceso por roles.
- Interfaz intuitiva con feedback visual.

### 🚫 Requerimientos No Funcionales
- Uso de SQL Server para persistencia de datos.
- Aplicación de escritorio con WPF (Windows Presentation Foundation).
- Arquitectura MVVM para mantener separación de responsabilidades.
- Buen rendimiento incluso con cientos de registros.

---

## 👨‍💻 Manual de Usuario

1. **Inicio de sesión** con control de acceso.
2. **Navegación** intuitiva entre módulos: Clientes, Vehículos, Servicios, Empleados, etc.
3. **Agregar, editar y eliminar** registros desde cada vista.
4. **Visualización de datos** en DataGrids con filtros y acciones.
5. **Interfaz limpia**, con animaciones suaves, colores amigables y accesibilidad.

> 📸 *Capturas de pantalla disponibles en la carpeta `/Capturas` del repositorio.*

---

## 🧱 Documentación Técnica

### 🏗️ Arquitectura
- Basada en el patrón **MVVM (Model-View-ViewModel)**.
- Componentes desacoplados para mejor mantenimiento.
- Estilos y componentes reutilizables mediante `CustomControls`.

### 🗃️ Base de Datos
- SQL Server con múltiples tablas relacionadas:
  - `Clientes`
  - `Vehiculos`
  - `Empleados`
  - `Ordenes`
  - `Servicios`

> Ver diseño completo en `/DiagramaBD.drawio` o `/Diagramas/dbdiagram.sql`.

### 🧩 Instalación

1. Clonar el repositorio:
   ```bash
   git clone https://github.com/tu_usuario/sitauto.git
