-- Script para mejorar el sistema de permisos
-- Autor: Cascade AI
-- Fecha: 2025-04-21

-- 1. Crear tabla PermissionRoutes para definir permisos de rutas
PRINT 'Verificando si existe la tabla PermissionRoutes...'
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PermissionRoutes]') AND type in (N'U'))
BEGIN
    PRINT 'Creando tabla PermissionRoutes...'
    CREATE TABLE [dbo].[PermissionRoutes](
        [Id] [uniqueidentifier] NOT NULL,
        [PermissionId] [uniqueidentifier] NOT NULL,
        [RouteId] [uniqueidentifier] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](256) NOT NULL,
        [LastModifiedAt] [datetime2](7) NULL,
        [LastModifiedBy] [nvarchar](256) NULL,
        CONSTRAINT [PK_PermissionRoutes] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_PermissionRoutes_Permissions_PermissionId] FOREIGN KEY([PermissionId]) REFERENCES [dbo].[Permissions] ([Id]),
        CONSTRAINT [FK_PermissionRoutes_Routes_RouteId] FOREIGN KEY([RouteId]) REFERENCES [dbo].[Routes] ([Id])
    );

    PRINT 'Tabla PermissionRoutes creada correctamente'
END
ELSE
BEGIN
    PRINT 'La tabla PermissionRoutes ya existe'
END

-- 2. Estandarizar la nomenclatura de permisos (cambiar routes.* a Routes.*)
PRINT 'Estandarizando la nomenclatura de permisos...'

-- Obtener el ID del rol Admin
DECLARE @AdminRoleId UNIQUEIDENTIFIER;
SELECT @AdminRoleId = Id FROM Roles WHERE Name = 'Admin';

IF @AdminRoleId IS NULL
BEGIN
    PRINT 'Error: No se encontró el rol Admin'
    RETURN
END

-- Crear permiso Routes.View si no existe
PRINT 'Verificando permiso Routes.View...'
DECLARE @RoutesViewId UNIQUEIDENTIFIER;
SELECT @RoutesViewId = Id FROM Permissions WHERE Name = 'Routes.View';

IF @RoutesViewId IS NULL
BEGIN
    SET @RoutesViewId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, IsActive, CreatedAt, CreatedBy)
    VALUES (@RoutesViewId, 'Routes.View', 'Permiso para ver rutas', 1, GETUTCDATE(), 'System');

    INSERT INTO RolePermissions (RoleId, PermissionId, IsActive, CreatedAt, CreatedBy)
    VALUES (@AdminRoleId, @RoutesViewId, 1, GETUTCDATE(), 'System');

    PRINT 'Permiso Routes.View creado y asignado al rol Admin'
END
ELSE
BEGIN
    PRINT 'El permiso Routes.View ya existe'
END

-- Crear permiso Routes.Create si no existe
PRINT 'Verificando permiso Routes.Create...'
DECLARE @RoutesCreateId UNIQUEIDENTIFIER;
SELECT @RoutesCreateId = Id FROM Permissions WHERE Name = 'Routes.Create';

IF @RoutesCreateId IS NULL
BEGIN
    SET @RoutesCreateId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, IsActive, CreatedAt, CreatedBy)
    VALUES (@RoutesCreateId, 'Routes.Create', 'Permiso para crear rutas', 1, GETUTCDATE(), 'System');

    INSERT INTO RolePermissions (RoleId, PermissionId, IsActive, CreatedAt, CreatedBy)
    VALUES (@AdminRoleId, @RoutesCreateId, 1, GETUTCDATE(), 'System');

    PRINT 'Permiso Routes.Create creado y asignado al rol Admin'
END
ELSE
BEGIN
    PRINT 'El permiso Routes.Create ya existe'
END

-- Crear permiso Routes.Edit si no existe
PRINT 'Verificando permiso Routes.Edit...'
DECLARE @RoutesEditId UNIQUEIDENTIFIER;
SELECT @RoutesEditId = Id FROM Permissions WHERE Name = 'Routes.Edit';

IF @RoutesEditId IS NULL
BEGIN
    SET @RoutesEditId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, IsActive, CreatedAt, CreatedBy)
    VALUES (@RoutesEditId, 'Routes.Edit', 'Permiso para editar rutas', 1, GETUTCDATE(), 'System');

    INSERT INTO RolePermissions (RoleId, PermissionId, IsActive, CreatedAt, CreatedBy)
    VALUES (@AdminRoleId, @RoutesEditId, 1, GETUTCDATE(), 'System');

    PRINT 'Permiso Routes.Edit creado y asignado al rol Admin'
END
ELSE
BEGIN
    PRINT 'El permiso Routes.Edit ya existe'
END

-- Crear permiso Routes.Delete si no existe
PRINT 'Verificando permiso Routes.Delete...'
DECLARE @RoutesDeleteId UNIQUEIDENTIFIER;
SELECT @RoutesDeleteId = Id FROM Permissions WHERE Name = 'Routes.Delete';

IF @RoutesDeleteId IS NULL
BEGIN
    SET @RoutesDeleteId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, IsActive, CreatedAt, CreatedBy)
    VALUES (@RoutesDeleteId, 'Routes.Delete', 'Permiso para eliminar rutas', 1, GETUTCDATE(), 'System');

    INSERT INTO RolePermissions (RoleId, PermissionId, IsActive, CreatedAt, CreatedBy)
    VALUES (@AdminRoleId, @RoutesDeleteId, 1, GETUTCDATE(), 'System');

    PRINT 'Permiso Routes.Delete creado y asignado al rol Admin'
END
ELSE
BEGIN
    PRINT 'El permiso Routes.Delete ya existe'
END

-- 3. Crear permiso específico para asignar rutas a módulos
PRINT 'Verificando permiso Routes.AssignToModule...'
DECLARE @RoutesAssignToModuleId UNIQUEIDENTIFIER;
SELECT @RoutesAssignToModuleId = Id FROM Permissions WHERE Name = 'Routes.AssignToModule';

IF @RoutesAssignToModuleId IS NULL
BEGIN
    SET @RoutesAssignToModuleId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, IsActive, CreatedAt, CreatedBy)
    VALUES (@RoutesAssignToModuleId, 'Routes.AssignToModule', 'Permiso para asignar rutas a módulos', 1, GETUTCDATE(), 'System');

    INSERT INTO RolePermissions (RoleId, PermissionId, IsActive, CreatedAt, CreatedBy)
    VALUES (@AdminRoleId, @RoutesAssignToModuleId, 1, GETUTCDATE(), 'System');

    PRINT 'Permiso Routes.AssignToModule creado y asignado al rol Admin'
END
ELSE
BEGIN
    PRINT 'El permiso Routes.AssignToModule ya existe'
END

-- 4. Crear módulo "Sin Asignar" si no existe
PRINT 'Verificando si existe el módulo "Sin Asignar"...'
DECLARE @UnassignedModuleId UNIQUEIDENTIFIER;
SELECT @UnassignedModuleId = Id FROM Modules WHERE Name = 'Sin Asignar';

IF @UnassignedModuleId IS NULL
BEGIN
    SET @UnassignedModuleId = NEWID();
    INSERT INTO Modules (Id, Name, Description, Route, Icon, DisplayOrder, IsEnabled, IsActive, CreatedAt, CreatedBy)
    VALUES (@UnassignedModuleId, 'Sin Asignar', 'Módulo para rutas sin asignación', '/unassigned', 'fa-question-circle', 9999, 1, 1, GETUTCDATE(), 'System');

    PRINT 'Módulo "Sin Asignar" creado correctamente'

    -- Asignar el permiso Modules.View al módulo "Sin Asignar"
    DECLARE @ModulesViewId UNIQUEIDENTIFIER;
    SELECT @ModulesViewId = Id FROM Permissions WHERE Name = 'Modules.View';

    IF @ModulesViewId IS NOT NULL
    BEGIN
        DECLARE @PermissionModuleId UNIQUEIDENTIFIER = NEWID();
        INSERT INTO PermissionModules (Id, PermissionId, ModuleId, CreatedAt, CreatedBy)
        VALUES (@PermissionModuleId, @ModulesViewId, @UnassignedModuleId, GETUTCDATE(), 'System');

        PRINT 'Permiso Modules.View asignado al módulo "Sin Asignar"'
    END
END
ELSE
BEGIN
    PRINT 'El módulo "Sin Asignar" ya existe'
END

-- 5. Asignar rutas sin módulo al módulo "Sin Asignar"
PRINT 'Asignando rutas sin módulo al módulo "Sin Asignar"...'
UPDATE Routes
SET ModuleId = @UnassignedModuleId,
    LastModifiedAt = GETUTCDATE(),
    LastModifiedBy = 'System'
WHERE ModuleId IS NULL OR ModuleId = '00000000-0000-0000-0000-000000000000';

PRINT 'Rutas sin módulo asignadas al módulo "Sin Asignar"'

-- 6. Inicializar la tabla PermissionRoutes con datos básicos
PRINT 'Inicializando la tabla PermissionRoutes con datos básicos...'
IF EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PermissionRoutes]') AND type in (N'U'))
BEGIN
    -- Verificar si ya hay datos en la tabla
    IF NOT EXISTS (SELECT 1 FROM PermissionRoutes)
    BEGIN
        INSERT INTO PermissionRoutes (Id, PermissionId, RouteId, CreatedAt, CreatedBy)
        SELECT 
            NEWID(),
            @RoutesViewId,
            Id,
            GETUTCDATE(),
            'System'
        FROM Routes
        WHERE IsActive = 1;

        PRINT 'Permisos básicos asignados a todas las rutas'
    END
    ELSE
    BEGIN
        PRINT 'La tabla PermissionRoutes ya contiene datos'
    END
END

PRINT 'Script ejecutado correctamente'
