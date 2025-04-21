-- Script para insertar permisos de rutas y asignarlos al rol de Administrador

-- ID del rol de Administrador
DECLARE @AdminRoleId UNIQUEIDENTIFIER = 'D7E350E8-5FB7-4517-B8DA-6F602D66A3A9';

-- Insertar permisos para rutas
DECLARE @RoutesViewId UNIQUEIDENTIFIER = NEWID();
DECLARE @RoutesCreateId UNIQUEIDENTIFIER = NEWID();
DECLARE @RoutesEditId UNIQUEIDENTIFIER = NEWID();
DECLARE @RoutesDeleteId UNIQUEIDENTIFIER = NEWID();

-- Insertar permisos
INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (@RoutesViewId, 'Routes.View', 'Permiso para ver rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (@RoutesCreateId, 'Routes.Create', 'Permiso para crear rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (@RoutesEditId, 'Routes.Edit', 'Permiso para editar rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (@RoutesDeleteId, 'Routes.Delete', 'Permiso para eliminar rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');

-- Asignar permisos al rol de Administrador
INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (NEWID(), @AdminRoleId, @RoutesViewId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (NEWID(), @AdminRoleId, @RoutesCreateId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (NEWID(), @AdminRoleId, @RoutesEditId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');

INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
VALUES 
    (NEWID(), @AdminRoleId, @RoutesDeleteId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
