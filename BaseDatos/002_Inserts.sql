-- =====================================================================
-- Script de inserts IDEMPOTENTE para Docker
-- Este script puede ejecutarse multiples veces sin crear duplicados.
-- Cada bloque verifica si el dato ya existe antes de insertarlo.
-- =====================================================================
USE DBVENTA
GO

-- ROLES
IF NOT EXISTS (SELECT 1 FROM Rol WHERE descripcion = 'Administrador')
BEGIN
    INSERT INTO Rol(descripcion, esActivo) VALUES ('Administrador', 1), ('Empleado', 1), ('Supervisor', 1)
END
GO

-- USUARIO ADMINISTRADOR
IF NOT EXISTS (SELECT 1 FROM Usuario WHERE correo = 'admin@example.com')
BEGIN
    INSERT INTO Usuario(nombre, correo, telefono, idRol, urlFoto, nombreFoto, clave, esActivo)
    VALUES ('Administrador', 'admin@example.com', '0000000000', 1, '', '', 'a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3', 1)
END
GO

-- CONFIGURACION FIREBASE
IF NOT EXISTS (SELECT 1 FROM Configuracion WHERE recurso = 'FireBase_Storage')
BEGIN
    INSERT INTO Configuracion(recurso, propiedad, valor) VALUES
        ('FireBase_Storage', 'email', ''),
        ('FireBase_Storage', 'clave', ''),
        ('FireBase_Storage', 'ruta', ''),
        ('FireBase_Storage', 'api_key', ''),
        ('FireBase_Storage', 'carpeta_usuario', 'IMAGENES_USUARIO'),
        ('FireBase_Storage', 'carpeta_producto', 'IMAGENES_PRODUCTO'),
        ('FireBase_Storage', 'carpeta_logo', 'IMAGENES_LOGO')
END
GO

-- CONFIGURACION CORREO
IF NOT EXISTS (SELECT 1 FROM Configuracion WHERE recurso = 'Servicio_Correo')
BEGIN
    INSERT INTO Configuracion(recurso, propiedad, valor) VALUES
        ('Servicio_Correo', 'correo', ''),
        ('Servicio_Correo', 'clave', ''),
        ('Servicio_Correo', 'alias', 'MiTienda.com'),
        ('Servicio_Correo', 'host', 'smtp.gmail.com'),
        ('Servicio_Correo', 'puerto', '587')
END
GO

-- NEGOCIO
IF NOT EXISTS (SELECT 1 FROM Negocio WHERE idNegocio = 1)
BEGIN
    INSERT INTO Negocio(idNegocio, urlLogo, nombreLogo, numeroDocumento, nombre, correo, direccion, telefono, porcentajeImpuesto, simboloMoneda)
    VALUES (1, '', '', '0000000001', 'Mi Tienda Online', 'contacto@mitienda.com', 'Av. Principal 123', '0999999999', 12, '$')
END
GO

-- CATEGORIAS
IF NOT EXISTS (SELECT 1 FROM Categoria WHERE descripcion = 'Computadoras')
BEGIN
    INSERT INTO Categoria(descripcion, esActivo) VALUES
        ('Computadoras', 1), ('Laptops', 1), ('Teclados', 1), ('Monitores', 1), ('Microfonos', 1)
END
GO

-- TIPOS DE DOCUMENTO DE VENTA
IF NOT EXISTS (SELECT 1 FROM TipoDocumentoVenta WHERE descripcion = 'Boleta')
BEGIN
    INSERT INTO TipoDocumentoVenta(descripcion, esActivo) VALUES ('Boleta', 1), ('Factura', 1)
END
GO

-- NUMERO CORRELATIVO
IF NOT EXISTS (SELECT 1 FROM NumeroCorrelativo WHERE gestion = 'venta')
BEGIN
    INSERT INTO NumeroCorrelativo(ultimoNumero, cantidadDigitos, gestion, fechaActualizacion)
    VALUES (0, 6, 'venta', GETDATE())
END
GO

-- MENU
IF NOT EXISTS (SELECT 1 FROM Menu WHERE descripcion = 'DashBoard')
BEGIN
    -- Menu raiz
    INSERT INTO Menu(descripcion, icono, controlador, paginaAccion, esActivo)
    VALUES ('DashBoard', 'fas fa-fw fa-tachometer-alt', 'DashBoard', 'Index', 1)

    -- Menus padre (sin controlador)
    INSERT INTO Menu(descripcion, icono, esActivo) VALUES
        ('Administracion', 'fas fa-fw fa-cog', 1),
        ('Inventario', 'fas fa-fw fa-clipboard-list', 1),
        ('Ventas', 'fas fa-fw fa-tags', 1),
        ('Reportes', 'fas fa-fw fa-chart-area', 1)

    -- Submenus de Administracion (idMenuPadre = 2)
    INSERT INTO Menu(descripcion, idMenuPadre, controlador, paginaAccion, esActivo) VALUES
        ('Usuarios', 2, 'Usuario', 'Index', 1),
        ('Negocio', 2, 'Negocio', 'Index', 1)

    -- Submenus de Inventario (idMenuPadre = 3)
    INSERT INTO Menu(descripcion, idMenuPadre, controlador, paginaAccion, esActivo) VALUES
        ('Categorias', 3, 'Categoria', 'Index', 1),
        ('Productos', 3, 'Producto', 'Index', 1)

    -- Submenus de Ventas (idMenuPadre = 4)
    INSERT INTO Menu(descripcion, idMenuPadre, controlador, paginaAccion, esActivo) VALUES
        ('Nueva Venta', 4, 'Venta', 'NuevaVenta', 1),
        ('Historial Venta', 4, 'Venta', 'HistorialVenta', 1)

    -- Submenus de Reportes (idMenuPadre = 5)
    INSERT INTO Menu(descripcion, idMenuPadre, controlador, paginaAccion, esActivo) VALUES
        ('Reporte de Ventas', 5, 'Reporte', 'Index', 1)

    -- Enlazar menus raiz con si mismos como padre
    UPDATE Menu SET idMenuPadre = idMenu WHERE idMenuPadre IS NULL
END
GO

-- ROL MENU (permisos)
IF NOT EXISTS (SELECT 1 FROM RolMenu WHERE idRol = 1)
BEGIN
    INSERT INTO RolMenu(idRol, idMenu, esActivo) VALUES (1,1,1),(1,6,1),(1,7,1),(1,8,1),(1,9,1),(1,10,1),(1,11,1),(1,12,1)
    INSERT INTO RolMenu(idRol, idMenu, esActivo) VALUES (2,10,1),(2,11,1)
    INSERT INTO RolMenu(idRol, idMenu, esActivo) VALUES (3,8,1),(3,9,1),(3,10,1),(3,11,1)
END
GO
