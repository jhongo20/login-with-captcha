-- Script para agregar permisos de módulos y asignarlos al rol Admin

-- Declarar variables para los IDs
DECLARE @AdminRoleId UNIQUEIDENTIFIER = 'D7E350E8-5FB7-4517-B8DA-6F602D66A3A9';
DECLARE @ModulesViewId UNIQUEIDENTIFIER = 'C4F907DB-0F34-4610-B3CC-9FD1C4D323E7';
DECLARE @ModulesCreateId UNIQUEIDENTIFIER = '7B8E8C2F-D39A-4B1A-B11E-0E39D3B7D8F3';
DECLARE @ModulesEditId UNIQUEIDENTIFIER = 'E5D4C4F4-4A4F-4A4F-B0D1-302C9D40E00F';
DECLARE @ModulesDeleteId UNIQUEIDENTIFIER = 'F8B2C5D5-5A5A-5A5A-C0E2-413D4E51F11F';
DECLARE @CurrentDateTime DATETIME2 = GETUTCDATE();

-- Insertar permisos para módulos
INSERT INTO [Permissions] ([Id], [Name], [Description], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy], [IsActive])
VALUES
    (@ModulesViewId, 'Modules.View', 'Permiso para ver módulos', @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1),
    (@ModulesCreateId, 'Modules.Create', 'Permiso para crear módulos', @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1),
    (@ModulesEditId, 'Modules.Edit', 'Permiso para editar módulos', @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1),
    (@ModulesDeleteId, 'Modules.Delete', 'Permiso para eliminar módulos', @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1);

-- Asignar permisos al rol Admin
INSERT INTO [RolePermissions] ([Id], [RoleId], [PermissionId], [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy], [IsActive])
VALUES
    (NEWID(), @AdminRoleId, @ModulesViewId, @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1),
    (NEWID(), @AdminRoleId, @ModulesCreateId, @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1),
    (NEWID(), @AdminRoleId, @ModulesEditId, @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1),
    (NEWID(), @AdminRoleId, @ModulesDeleteId, @CurrentDateTime, 'System', @CurrentDateTime, 'System', 1);
