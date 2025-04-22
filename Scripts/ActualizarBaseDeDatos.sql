-- Script para actualizar la base de datos con los cambios necesarios para la relación entre módulos y roles

-- Verificar si existe la tabla PermissionModules
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PermissionModules')
BEGIN
    PRINT 'Creando tabla PermissionModules...'
    
    CREATE TABLE [dbo].[PermissionModules](
        [Id] [uniqueidentifier] NOT NULL,
        [PermissionId] [uniqueidentifier] NOT NULL,
        [ModuleId] [uniqueidentifier] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](50) NOT NULL,
        [LastModifiedAt] [datetime2](7) NULL,
        [LastModifiedBy] [nvarchar](50) NULL,
        CONSTRAINT [PK_PermissionModules] PRIMARY KEY CLUSTERED 
        (
            [Id] ASC
        ),
        CONSTRAINT [IX_PermissionModules_PermissionId_ModuleId] UNIQUE NONCLUSTERED 
        (
            [PermissionId] ASC,
            [ModuleId] ASC
        ),
        CONSTRAINT [FK_PermissionModules_Permissions_PermissionId] FOREIGN KEY([PermissionId])
            REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PermissionModules_Modules_ModuleId] FOREIGN KEY([ModuleId])
            REFERENCES [dbo].[Modules] ([Id]) ON DELETE CASCADE
    )
    
    -- Crear índice para mejorar el rendimiento
    CREATE NONCLUSTERED INDEX [IX_PermissionModules_ModuleId] ON [dbo].[PermissionModules]
    (
        [ModuleId] ASC
    )
    
    PRINT 'Tabla PermissionModules creada correctamente'
END
ELSE
BEGIN
    PRINT 'La tabla PermissionModules ya existe'
END

-- Verificar si existe el permiso Modules.View
IF NOT EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Modules.View')
BEGIN
    PRINT 'Creando permiso Modules.View...'
    
    DECLARE @PermissionId uniqueidentifier = NEWID()
    
    -- Insertar el permiso
    INSERT INTO [dbo].[Permissions] (
        [Id], 
        [Name], 
        [Description], 
        [IsActive], 
        [CreatedAt], 
        [CreatedBy], 
        [LastModifiedAt], 
        [LastModifiedBy]
    )
    VALUES (
        @PermissionId, 
        'Modules.View', 
        'Permite ver módulos en el sistema', 
        1, 
        GETUTCDATE(), 
        'System', 
        NULL, 
        NULL
    )
    
    -- Asignar el permiso al rol Admin
    INSERT INTO [dbo].[RolePermissions] (
        [RoleId], 
        [PermissionId], 
        [IsActive], 
        [CreatedAt], 
        [CreatedBy], 
        [LastModifiedAt], 
        [LastModifiedBy]
    )
    SELECT 
        [Id], 
        @PermissionId, 
        1, 
        GETUTCDATE(), 
        'System', 
        NULL, 
        NULL
    FROM [dbo].[Roles]
    WHERE [Name] = 'Admin'
    
    PRINT 'Permiso Modules.View creado y asignado al rol Admin correctamente'
END
ELSE
BEGIN
    PRINT 'El permiso Modules.View ya existe'
END

-- Asignar módulos existentes al rol Admin
PRINT 'Asignando módulos al rol Admin...'

-- Obtener el ID del rol Admin
DECLARE @AdminRoleId uniqueidentifier
SELECT @AdminRoleId = [Id] FROM [dbo].[Roles] WHERE [Name] = 'Admin'

-- Obtener el ID del permiso Modules.View
DECLARE @ModulesViewPermissionId uniqueidentifier
SELECT @ModulesViewPermissionId = [Id] FROM [dbo].[Permissions] WHERE [Name] = 'Modules.View'

-- Asignar todos los módulos al rol Admin
INSERT INTO [dbo].[PermissionModules] (
    [Id],
    [PermissionId],
    [ModuleId],
    [CreatedAt],
    [CreatedBy],
    [LastModifiedAt],
    [LastModifiedBy]
)
SELECT 
    NEWID(),
    @ModulesViewPermissionId,
    m.[Id],
    GETUTCDATE(),
    'System',
    NULL,
    NULL
FROM [dbo].[Modules] m
WHERE NOT EXISTS (
    SELECT 1 
    FROM [dbo].[PermissionModules] pm 
    WHERE pm.[ModuleId] = m.[Id] AND pm.[PermissionId] = @ModulesViewPermissionId
)

PRINT 'Proceso de actualización completado correctamente'
