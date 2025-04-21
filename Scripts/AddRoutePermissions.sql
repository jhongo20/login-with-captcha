-- Script para agregar permisos de rutas y asignarlos al rol de Administrador

-- ID del rol de Administrador
DECLARE @AdminRoleId UNIQUEIDENTIFIER = 'D7E350E8-5FB7-4517-B8DA-6F602D66A3A9';

-- Variables para los IDs de los permisos
DECLARE @RoutesViewId UNIQUEIDENTIFIER;
DECLARE @RoutesCreateId UNIQUEIDENTIFIER;
DECLARE @RoutesEditId UNIQUEIDENTIFIER;
DECLARE @RoutesDeleteId UNIQUEIDENTIFIER;

-- Verificar si los permisos ya existen y obtener sus IDs
SELECT @RoutesViewId = Id FROM Permissions WHERE Name = 'Routes.View';
SELECT @RoutesCreateId = Id FROM Permissions WHERE Name = 'Routes.Create';
SELECT @RoutesEditId = Id FROM Permissions WHERE Name = 'Routes.Edit';
SELECT @RoutesDeleteId = Id FROM Permissions WHERE Name = 'Routes.Delete';

-- Insertar permisos solo si no existen
IF @RoutesViewId IS NULL
BEGIN
    SET @RoutesViewId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesViewId, 'Routes.View', 'Permiso para ver rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF @RoutesCreateId IS NULL
BEGIN
    SET @RoutesCreateId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesCreateId, 'Routes.Create', 'Permiso para crear rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF @RoutesEditId IS NULL
BEGIN
    SET @RoutesEditId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesEditId, 'Routes.Edit', 'Permiso para editar rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF @RoutesDeleteId IS NULL
BEGIN
    SET @RoutesDeleteId = NEWID();
    INSERT INTO Permissions (Id, Name, Description, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (@RoutesDeleteId, 'Routes.Delete', 'Permiso para eliminar rutas', GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

-- Verificar si los permisos ya están asignados al rol de Administrador
IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesViewId AND IsActive = 1)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesViewId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesCreateId AND IsActive = 1)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesCreateId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesEditId AND IsActive = 1)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesEditId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @AdminRoleId AND PermissionId = @RoutesDeleteId AND IsActive = 1)
BEGIN
    INSERT INTO RolePermissions (Id, RoleId, PermissionId, IsActive, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy)
    VALUES (NEWID(), @AdminRoleId, @RoutesDeleteId, 1, GETUTCDATE(), 'System', GETUTCDATE(), 'System');
END

-- Insertar algunas rutas de ejemplo para el módulo Dashboard
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
