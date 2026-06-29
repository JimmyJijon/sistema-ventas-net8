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

## Servicio de Correo Electrónico (MailDev)

El sistema incluye un servidor de correo SMTP local llamado **MailDev** que intercepta todos los correos enviados por la aplicación (recuperación de contraseña, notificaciones, etc.) y los muestra en una bandeja de entrada visual. **Los correos nunca salen a internet**, lo que hace este entorno completamente seguro para pruebas.

### ¿Por qué MailDev?
El archivo `.env` (que contiene credenciales reales de SMTP) **no se sube al repositorio** por razones de seguridad. Gracias a MailDev, el sistema funciona con correos sin necesidad de configurar ninguna cuenta externa.

### Acceso a la bandeja de entrada de MailDev
Una vez levantados los contenedores con `docker compose up -d`, puedes ver todos los correos interceptados en tu navegador:

* **En local:** [http://localhost:1080](http://localhost:1080)
* **En servidor en la nube:** `http://<IP-PUBLICA-DEL-SERVIDOR>:1080`

### ¿Cómo usar un servicio de correo real (Gmail, etc.)?
Si deseas que los correos se envíen de verdad a los destinatarios, debes editar tu archivo `.env` en la raíz del proyecto y completar las siguientes variables con tus credenciales reales:

```env
SMTP_CORREO=tu-correo@gmail.com
SMTP_CLAVE=tu-contraseña-de-aplicacion
SMTP_ALIAS=Nombre que aparece en el correo
```

> **Nota:** Para Gmail, debes usar una **Contraseña de Aplicación** (no tu contraseña normal). Puedes generarla en [Seguridad de tu cuenta de Google](https://myaccount.google.com/security) activando la verificación en dos pasos y luego en *Contraseñas de aplicación*.

Luego reinicia los contenedores para que el inicializador de base de datos inyecte las nuevas credenciales:
```bash
docker compose down
docker compose up -d
```

## Solución para Servidores con poca RAM (fake_meminfo)
SQL Server exige un mínimo de **2 GB de memoria RAM** para poder iniciar. Si se intenta levantar el contenedor en servidores (como instancias micro de Google Cloud) o máquinas virtuales con menos de 2 GB, el contenedor de la base de datos se detendrá instantáneamente.

Para solucionar esto de manera transparente, el repositorio incluye los archivos `sysinfo.c` y `fake_meminfo`.
* Durante la construcción de la imagen de SQL Server (definida en el `docker-compose.yml`), este pequeño programa en C se compila y se inyecta en el contenedor.
* Su único trabajo es interceptar las consultas del sistema operativo y decirle a SQL Server que **siempre hay 4 GB de RAM disponibles**, engañando la validación y permitiendo que la base de datos funcione perfectamente en entornos limitados.
* **Nota de seguridad:** Estos archivos no contienen datos sensibles, variables de entorno ni credenciales. Son puramente un *bypass* técnico seguro y necesario para el despliegue.

