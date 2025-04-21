-- Script para insertar permisos de rutas y asignarlos al rol de Administrador

-- ID del rol de Administrador
DECLARE @AdminRoleId UNIQUEIDENTIFIER = 'D7E350E8-5FB7-4517-B8DA-6F602D66A3A9';

-- Crear IDs para los permisos de rutas
DECLARE @RoutesViewId UNIQUEIDENTIFIER = 'D1E2F3A4-B5C6-4D7E-8F9A-0B1C2D3E4F5A';
DECLARE @RoutesCreateId UNIQUEIDENTIFIER = 'A1B2C3D4-E5F6-4A5B-6C7D-8E9F0A1B2C3D';
DECLARE @RoutesEditId UNIQUEIDENTIFIER = 'F1E2D3C4-B5A6-4F7E-8D9C-0B1A2C3D4E5F';
DECLARE @RoutesDeleteId UNIQUEIDENTIFIER = '1A2B3C4D-5E6F-4A7B-8C9D-0E1F2A3B4C5D';

-- Insertar permisos si no existen
IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'routes.view')
BEGIN
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesViewId, 'routes.view', 'Permiso para ver rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'routes.create')
BEGIN
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesCreateId, 'routes.create', 'Permiso para crear rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'routes.edit')
BEGIN
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesEditId, 'routes.edit', 'Permiso para editar rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM Permissions WHERE Name = 'routes.delete')
BEGIN
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesDeleteId, 'routes.delete', 'Permiso para eliminar rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

-- Asignar permisos al rol de Administrador si no están asignados
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesViewId)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesViewId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesCreateId)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesCreateId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesEditId)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesEditId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesDeleteId)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesDeleteId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

-- Insertar rutas de ejemplo para el módulo Dashboard
DECLARE @DashboardModuleId UNIQUEIDENTIFIER;

-- Obtener el ID del módulo Dashboard
SELECT @DashboardModuleId = Id FROM Modules WHERE Name = 'Dashboard';

-- Si existe el módulo Dashboard, insertar rutas de ejemplo
IF @DashboardModuleId IS NOT NULL
BEGIN
    -- Verificar si las rutas ya existen
    IF NOT EXISTS (SELECT 1 FROM Routes WHERE Path = '/api/dashboard/stats' AND HttpMethod = 'GET')
    BEGIN
        INSERT INTO Routes (Id, Name, Description, Path, HttpMethod, DisplayOrder, RequiresAuth, IsEnabled, IsActive, ModuleId, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
        VALUES (NEWID(), 'Obtener estadísticas', 'Obtiene las estadísticas del dashboard', '/api/dashboard/stats', 'GET', 1, 1, 1, 1, @DashboardModuleId, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
    END

    IF NOT EXISTS (SELECT 1 FROM Routes WHERE Path = '/api/dashboard/activity' AND HttpMethod = 'GET')
    BEGIN
        INSERT INTO Routes (Id, Name, Description, Path, HttpMethod, DisplayOrder, RequiresAuth, IsEnabled, IsActive, ModuleId, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
        VALUES (NEWID(), 'Obtener actividad reciente', 'Obtiene la actividad reciente del sistema', '/api/dashboard/activity', 'GET', 2, 1, 1, 1, @DashboardModuleId, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
    END
END
