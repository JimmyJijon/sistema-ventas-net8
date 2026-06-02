# Sistema de Ventas en .NET 8 

Sistema integral de punto de venta y administración de inventario, desarrollado con **ASP.NET Core 8 MVC**, diseñado bajo el patrón **Modelo-Vista-Controlador (MVC)** y estructurado en **Arquitectura de N-Capas**.

## Alcance del Sistema
El sistema permite la administración completa de un negocio comercial, incluyendo:
* **DashBoard:** Gráficos y reportes visuales de ventas e inventario.
* **Módulo de Usuarios:** Gestión de roles (Administrador, Empleado, Supervisor) y accesos.
* **Módulo de Inventario:** Control de Categorías y Productos.
* **Módulo de Ventas:** Registro de ventas, generación de facturas/boletas y exportación a **PDF** (usando `DinkToPdf` y wkhtmltopdf nativo).
* **Reportes:** Historial y detalle de ventas exportables a Excel.
* **Negocio:** Configuración de logotipos, moneda, impuestos y correo electrónico.

## Tecnologías y Herramientas
* **Backend:** C# con .NET 8 (ASP.NET Core MVC).
* **Frontend:** HTML5, CSS3, JavaScript puro, Bootstrap y DataTables.
* **Base de Datos:** SQL Server 2022.
* **ORM:** Entity Framework Core.
* **Docker:** Contenerización del entorno completo (`docker-compose`).
* **PDF:** `DinkToPdf`.
* **Testing Local SMTP:** MailDev para pruebas de envío de correos.

## Arquitectura de Capas
El proyecto está fuertemente desacoplado para asegurar mantenibilidad y escalabilidad, dividido en los siguientes proyectos:
1. **SistemaVenta.Entity:** Contiene las entidades y modelos de la base de datos.
2. **SistemaVenta.DAL (Data Access Layer):** Capa de acceso a datos, implementa el patrón Repositorio y los contextos de Entity Framework.
3. **SistemaVenta.BLL (Business Logic Layer):** Capa de Lógica de Negocio. Aquí residen todas las reglas, validaciones y servicios del dominio, además de la integración con servicios externos como Firebase y envío de correos.
4. **SistemaVenta.IOC (Inversion of Control):** Capa dedicada a la inyección de dependencias. Centraliza los servicios para mantener la capa web limpia.
5. **SistemaVenta.AplicacionWeb:** La capa de presentación bajo el patrón **MVC**. Maneja los controladores, las vistas (Razor/HTML) y los recursos estáticos (wwwroot).

## Integración con Firebase Storage
El sistema utiliza Firebase Storage como un CDN externo para almacenar y servir las imágenes de forma eficiente, evitando sobrecargar la base de datos o el disco del servidor local con archivos binarios. Se integra mediante la API en la capa BLL para gestionar:
* Fotos de perfil de usuarios.
* Imágenes del catálogo de productos.
* Logotipo del negocio.

## Ejecución con Docker
El proyecto está preparado para levantarse con un solo comando usando Docker Compose. Esto iniciará la base de datos, insertará los datos semilla iniciales, levantará el servidor SMTP de pruebas y la aplicación web.

1. Asegúrate de tener tu archivo `.env` configurado en la raíz.
2. Abre una terminal y ejecuta:
```bash
docker compose up -d
```
3. La aplicación estará disponible en `http://localhost:5005` (o en tu IP de servidor si estás en la nube).

## Acceso por Defecto
El script de inicialización (`002_Inserts.sql`) crea automáticamente la base de datos y un administrador:

**Credenciales del Panel Web:**
* **Correo:** `admin@example.com`
* **Contraseña:** `123`

**Credenciales de Base de Datos (Si necesitas conectarte vía SSMS localmente):**
* **Servidor:** `localhost,14333`
* **Usuario:** `sa`
* **Contraseña:** *(La que hayas definido en tu archivo .env)*
