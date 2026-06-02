# =============================================================
# DOCKERFILE - Sistema de Ventas .NET 8
# Compilacion Multi-Etapa (Multi-Stage Build)
# =============================================================

# ---------------------------------------------------------------
# ETAPA 1: "build" - Compilar y publicar la aplicacion
# Usamos la imagen SDK completa de .NET 8 para compilar el codigo
# ---------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos los archivos .csproj de todos los proyectos de la solucion
# Esto aprovecha el cache de Docker: si los .csproj no cambian,
# Docker no volvera a descargar los paquetes NuGet innecesariamente
COPY SistemaVenta.Entity/SistemaVenta.Entity.csproj SistemaVenta.Entity/
COPY SistemaVenta.DAL/SistemaVenta.DAL.csproj SistemaVenta.DAL/
COPY SistemaVenta.BLL/SistemaVenta.BLL.csproj SistemaVenta.BLL/
COPY SistemaVenta.IOC/SistemaVenta.IOC.csproj SistemaVenta.IOC/
COPY SistemaVenta.AplicacionWeb/SistemaVenta.AplicacionWeb.csproj SistemaVenta.AplicacionWeb/

# Restauramos los paquetes NuGet de todos los proyectos
RUN dotnet restore SistemaVenta.AplicacionWeb/SistemaVenta.AplicacionWeb.csproj

# Ahora copiamos el resto del codigo fuente completo
COPY . .

# Compilamos y publicamos la aplicacion web en modo Release
# El resultado se guarda en la carpeta /app/publish
RUN dotnet publish SistemaVenta.AplicacionWeb/SistemaVenta.AplicacionWeb.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ---------------------------------------------------------------
# ETAPA 2: "final" - Imagen de produccion (mucho mas ligera)
# Solo usamos el runtime de ASP.NET 8, sin el SDK completo
# Esto hace la imagen final mucho mas pequeña y segura
# ---------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalamos las dependencias del sistema que necesita DinkToPdf
# (la libreria que usa el sistema para generar PDFs de las ventas)
# libwkhtmltox.so requiere estas librerias nativas en Debian (base de aspnet:8.0)
RUN apt-get update && apt-get install -y \
    libgdiplus \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libssl3 \
    libfontconfig1 \
    libfreetype6 \
    libjpeg62-turbo \
    libpng16-16 \
    libxcb1 \
    && rm -rf /var/lib/apt/lists/*

# Copiamos unicamente los archivos compilados desde la etapa "build"
COPY --from=build /app/publish .

# Registramos libwkhtmltox.so en el sistema para que DllImport (DinkToPdf) la encuentre.
# DinkToPdf usa P/Invoke con DllImport("libwkhtmltox"), lo cual requiere que el linker
# de Linux pueda resolver el nombre via las rutas estandar del sistema (/usr/local/lib, etc.).
# Copiar el .so a /usr/local/lib y correr ldconfig lo registra globalmente.
RUN cp /app/Utilidades/LibreriaPDF/libwkhtmltox.so /usr/local/lib/libwkhtmltox.so \
    && ldconfig

# Exponemos el puerto 5005 (puerto interno del contenedor)
EXPOSE 5005

# Variable de entorno para que ASP.NET Core escuche en el puerto correcto
ENV ASPNETCORE_URLS=http://+:5005
# Ruta adicional de busqueda de librerias (fallback de seguridad)
ENV LD_LIBRARY_PATH=/app/Utilidades/LibreriaPDF:$LD_LIBRARY_PATH

# Comando que se ejecuta cuando el contenedor arranca
ENTRYPOINT ["dotnet", "SistemaVenta.AplicacionWeb.dll"]
