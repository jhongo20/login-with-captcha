# Guía de Solución de Problemas: Rutas y Permisos

## Introducción

Esta guía proporciona soluciones a problemas comunes que pueden surgir al trabajar con el sistema de rutas y permisos en AuthSystem. Está diseñada para desarrolladores y administradores del sistema que necesiten diagnosticar y resolver problemas relacionados con la creación de rutas, asignación de rutas a roles y asignación de permisos a rutas.

## Problemas al Crear Rutas

### Problema: Error 500 al crear una ruta

**Síntoma**: Al intentar crear una nueva ruta a través del endpoint `POST /api/Routes`, se recibe un error 500 con el mensaje "Error al crear la ruta".

**Posibles causas**:
1. La propiedad `IsActive` no se está estableciendo correctamente
2. Hay un problema con la conexión a la base de datos
3. El módulo especificado no existe o no está activo

**Solución**:
1. Asegúrate de que la propiedad `IsActive` esté establecida a `true` en el objeto de la ruta
2. Verifica que la conexión a la base de datos esté funcionando correctamente
3. Confirma que el módulo especificado existe y está activo

**Implementación técnica**:
Si el problema persiste, considera utilizar ADO.NET directamente para la inserción en la base de datos en lugar de Entity Framework:

```csharp
string insertQuery = @"
    INSERT INTO Routes (
        Id, Name, Description, Path, HttpMethod, 
        DisplayOrder, RequiresAuth, IsEnabled, IsActive, 
        ModuleId, CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
    ) VALUES (
        @Id, @Name, @Description, @Path, @HttpMethod, 
        @DisplayOrder, @RequiresAuth, @IsEnabled, 1, 
        @ModuleId, @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
    )";

using (var connection = new SqlConnection(_connectionString))
{
    await connection.OpenAsync();
    using (var command = new SqlCommand(insertQuery, connection))
    {
        command.Parameters.AddWithValue("@Id", Guid.NewGuid());
        command.Parameters.AddWithValue("@Name", request.Name);
        // Añadir el resto de parámetros...
        
        await command.ExecuteNonQueryAsync();
    }
}
```

## Problemas al Asignar Rutas a Roles

### Problema: Error 500 al asignar una ruta a un rol

**Síntoma**: Al intentar asignar una ruta a un rol a través del endpoint `POST /api/Routes/assign`, se recibe un error 500.

**Posibles causas**:
1. La ruta o el rol no existen o no están activos
2. Ya existe una relación entre la ruta y el rol
3. El rol no tiene acceso al módulo que contiene la ruta
4. Hay un problema con la estructura de la tabla `RoleRoutes`

**Solución**:
1. Verifica que tanto la ruta como el rol existan y estén activos
2. Si la relación ya existe, actualízala en lugar de crear una nueva
3. Asegúrate de que el rol tenga acceso al módulo que contiene la ruta
4. Verifica que la tabla `RoleRoutes` tenga la estructura correcta, incluyendo la columna `IsActive`

**Implementación técnica**:
Utiliza ADO.NET para verificar y crear/actualizar la relación:

```csharp
// Verificar si ya existe la relación
string checkQuery = @"
    SELECT COUNT(*) FROM RoleRoutes 
    WHERE RouteId = @RouteId AND RoleId = @RoleId";

// Si existe, actualizar
string updateQuery = @"
    UPDATE RoleRoutes 
    SET IsActive = 1, 
        LastModifiedAt = @LastModifiedAt, 
        LastModifiedBy = @LastModifiedBy 
    WHERE RouteId = @RouteId AND RoleId = @RoleId";

// Si no existe, crear
string insertQuery = @"
    INSERT INTO RoleRoutes (
        Id, RoleId, RouteId, IsActive, 
        CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
    ) VALUES (
        @Id, @RoleId, @RouteId, 1, 
        @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
    )";
```

## Problemas al Asignar Permisos a Rutas

### Problema: Error "Invalid column name 'IsActive'" al asignar un permiso a una ruta

**Síntoma**: Al intentar asignar un permiso a una ruta a través del endpoint `POST /api/PermissionRoutes/assign/{routeId}/{permissionId}`, se recibe un error relacionado con la columna `IsActive`.

**Causa**: La tabla `PermissionRoutes` no tiene la columna `IsActive` que se está utilizando en el código.

**Solución**:
1. Añadir la columna `IsActive` a la tabla `PermissionRoutes`:

```sql
ALTER TABLE PermissionRoutes ADD IsActive bit NOT NULL DEFAULT 1
```

2. Modificar el código para utilizar ADO.NET directamente en lugar de Entity Framework:

```csharp
// Verificar si ya existe la relación
string checkQuery = @"
    SELECT COUNT(*) FROM PermissionRoutes 
    WHERE RouteId = @RouteId AND PermissionId = @PermissionId";

// Si existe, actualizar
string updateQuery = @"
    UPDATE PermissionRoutes 
    SET IsActive = 1, 
        LastModifiedAt = @LastModifiedAt, 
        LastModifiedBy = @LastModifiedBy 
    WHERE RouteId = @RouteId AND PermissionId = @PermissionId";

// Si no existe, crear
string insertQuery = @"
    INSERT INTO PermissionRoutes (
        Id, PermissionId, RouteId, IsActive, 
        CreatedAt, CreatedBy, LastModifiedAt, LastModifiedBy
    ) VALUES (
        @Id, @PermissionId, @RouteId, 1, 
        @CreatedAt, @CreatedBy, @LastModifiedAt, @LastModifiedBy
    )";
```

## Problemas al Revocar Permisos de Rutas

### Problema: Error "El permiso no está asignado a la ruta" al revocar un permiso

**Síntoma**: Al intentar revocar un permiso de una ruta a través del endpoint `DELETE /api/PermissionRoutes/revoke/{routeId}/{permissionId}`, se recibe un error indicando que el permiso no está asignado a la ruta.

**Posibles causas**:
1. La relación entre el permiso y la ruta no existe
2. La relación existe pero ya está inactiva (`IsActive = false`)
3. Hay un problema con la consulta que verifica la existencia de la relación

**Solución**:
1. Verifica que la relación exista en la base de datos
2. Asegúrate de que la consulta que verifica la existencia de la relación incluya la condición `IsActive = 1`
3. Utiliza ADO.NET para verificar y actualizar la relación:

```csharp
// Verificar si existe la relación y está activa
string checkQuery = @"
    SELECT COUNT(*) FROM PermissionRoutes 
    WHERE RouteId = @RouteId AND PermissionId = @PermissionId AND IsActive = 1";

// Desactivar la relación
string updateQuery = @"
    UPDATE PermissionRoutes 
    SET IsActive = 0, 
        LastModifiedAt = @LastModifiedAt, 
        LastModifiedBy = @LastModifiedBy 
    WHERE RouteId = @RouteId AND PermissionId = @PermissionId";
```

## Problemas de Rendimiento

### Problema: Lentitud al cargar rutas o verificar permisos

**Síntoma**: Las operaciones relacionadas con rutas o permisos son lentas, especialmente cuando hay muchas rutas o permisos en el sistema.

**Posibles causas**:
1. Falta de índices en las tablas de la base de datos
2. Consultas ineficientes
3. Carga excesiva de datos relacionados

**Solución**:
1. Añadir índices a las columnas clave:

```sql
CREATE INDEX IX_Routes_ModuleId ON Routes(ModuleId);
CREATE INDEX IX_RoleRoutes_RouteId ON RoleRoutes(RouteId);
CREATE INDEX IX_RoleRoutes_RoleId ON RoleRoutes(RoleId);
CREATE INDEX IX_PermissionRoutes_RouteId ON PermissionRoutes(RouteId);
CREATE INDEX IX_PermissionRoutes_PermissionId ON PermissionRoutes(PermissionId);
```

2. Optimizar las consultas para cargar solo los datos necesarios
3. Implementar caché para datos que no cambian frecuentemente

## Herramientas de Diagnóstico

### Scripts de Verificación

Utiliza estos scripts para verificar el estado de las rutas, roles y permisos en el sistema:

#### Verificar rutas activas
```sql
SELECT Id, Name, Path, HttpMethod, ModuleId, IsActive 
FROM Routes 
WHERE IsActive = 1
```

#### Verificar asignaciones de rutas a roles
```sql
SELECT rr.Id, r.Name AS RouteName, ro.Name AS RoleName, rr.IsActive 
FROM RoleRoutes rr
JOIN Routes r ON rr.RouteId = r.Id
JOIN Roles ro ON rr.RoleId = ro.Id
```

#### Verificar asignaciones de permisos a rutas
```sql
SELECT pr.Id, r.Name AS RouteName, p.Name AS PermissionName, pr.IsActive 
FROM PermissionRoutes pr
JOIN Routes r ON pr.RouteId = r.Id
JOIN Permissions p ON pr.PermissionId = p.Id
```

### Scripts PowerShell para Pruebas

Utiliza estos scripts PowerShell para probar los endpoints de rutas y permisos:

#### Probar la creación de rutas
```powershell
$token = "your-jwt-token"
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$routeJson = @{
    name = "Test Route"
    description = "Test route for troubleshooting"
    path = "/api/test/route"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = $true
    isEnabled = $true
    moduleId = "your-module-id"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Post -Body $routeJson -Headers $headers
```

#### Probar la asignación de rutas a roles
```powershell
$token = "your-jwt-token"
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

$assignJson = @{
    routeId = "your-route-id"
    roleId = "your-role-id"
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5031/api/Routes/assign" -Method Post -Body $assignJson -Headers $headers
```

## Contacto para Soporte

Si después de seguir esta guía sigues experimentando problemas, contacta al equipo de desarrollo:

- **Email**: support@authsystem.com
- **Jira**: [AuthSystem Project](https://authsystem.atlassian.net)
- **GitHub**: [AuthSystem Repository](https://github.com/authsystem/authsystem)
