# Guía de Pruebas: Asociación de Módulos a Roles

## Introducción

Esta guía proporciona instrucciones detalladas para probar la funcionalidad de asociación de módulos a roles en el sistema AuthSystem. Las pruebas cubren todos los aspectos de la funcionalidad, desde la creación de la base de datos hasta la interacción con la API.

## Requisitos Previos

- Instancia de SQL Server en ejecución
- Proyecto AuthSystem compilado y en ejecución
- Herramienta para pruebas de API (Postman, Swagger UI, etc.)
- Token JWT válido con el rol `Admin`

## Pruebas de Base de Datos

### 1. Verificar Creación de Tabla PermissionModules

1. Conectarse a la base de datos `AuthSystemNewDb`
2. Ejecutar la siguiente consulta:

```sql
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PermissionModules'
```

**Resultado esperado**: La consulta debe devolver una fila, indicando que la tabla existe.

### 2. Verificar Estructura de la Tabla PermissionModules

```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'PermissionModules'
```

**Resultado esperado**: La consulta debe devolver las columnas `Id`, `PermissionId`, `ModuleId`, `CreatedAt`, `CreatedBy`, `LastModifiedAt` y `LastModifiedBy` con los tipos de datos correctos.

### 3. Verificar Permiso Modules.View

```sql
SELECT * FROM Permissions WHERE Name = 'Modules.View'
```

**Resultado esperado**: La consulta debe devolver una fila con el permiso `Modules.View`.

### 4. Verificar Asignación del Permiso al Rol Admin

```sql
SELECT rp.* 
FROM RolePermissions rp
JOIN Permissions p ON rp.PermissionId = p.Id
JOIN Roles r ON rp.RoleId = r.Id
WHERE p.Name = 'Modules.View' AND r.Name = 'Admin'
```

**Resultado esperado**: La consulta debe devolver al menos una fila, indicando que el permiso está asignado al rol Admin.

## Pruebas de API

### 1. Obtener Módulos por Rol

#### Caso de Prueba: Obtener módulos para un rol existente

**Solicitud**:
```
GET /api/Modules/byRole/{roleId}
Authorization: Bearer {token}
```

**Resultado esperado**:
- Código de estado: 200 OK
- Cuerpo de respuesta: Array de objetos ModuleDto

#### Caso de Prueba: Obtener módulos para un rol inexistente

**Solicitud**:
```
GET /api/Modules/byRole/{roleId_inexistente}
Authorization: Bearer {token}
```

**Resultado esperado**:
- Código de estado: 404 Not Found
- Cuerpo de respuesta: Mensaje de error indicando que el rol no existe

### 2. Asignar Módulo a Rol

#### Caso de Prueba: Asignar un módulo a un rol (caso exitoso)

**Solicitud**:
```
POST /api/Modules/assign-to-role
Authorization: Bearer {token}
Content-Type: application/json

{
  "moduleId": "{moduleId}",
  "roleId": "{roleId}"
}
```

**Resultado esperado**:
- Código de estado: 200 OK
- Cuerpo de respuesta: Mensaje de confirmación

#### Caso de Prueba: Asignar un módulo ya asignado a un rol

**Solicitud**:
```
POST /api/Modules/assign-to-role
Authorization: Bearer {token}
Content-Type: application/json

{
  "moduleId": "{moduleId_ya_asignado}",
  "roleId": "{roleId}"
}
```

**Resultado esperado**:
- Código de estado: 400 Bad Request
- Cuerpo de respuesta: Mensaje indicando que el rol ya tiene acceso al módulo

#### Caso de Prueba: Asignar un módulo inexistente a un rol

**Solicitud**:
```
POST /api/Modules/assign-to-role
Authorization: Bearer {token}
Content-Type: application/json

{
  "moduleId": "{moduleId_inexistente}",
  "roleId": "{roleId}"
}
```

**Resultado esperado**:
- Código de estado: 404 Not Found
- Cuerpo de respuesta: Mensaje indicando que el módulo no existe

### 3. Revocar Módulo de Rol

#### Caso de Prueba: Revocar un módulo de un rol (caso exitoso)

**Solicitud**:
```
DELETE /api/Modules/revoke-from-role/{roleId}/{moduleId}
Authorization: Bearer {token}
```

**Resultado esperado**:
- Código de estado: 200 OK
- Cuerpo de respuesta: Mensaje de confirmación

#### Caso de Prueba: Revocar un módulo no asignado a un rol

**Solicitud**:
```
DELETE /api/Modules/revoke-from-role/{roleId}/{moduleId_no_asignado}
Authorization: Bearer {token}
```

**Resultado esperado**:
- Código de estado: 400 Bad Request
- Cuerpo de respuesta: Mensaje indicando que el rol no tiene acceso al módulo

## Pruebas de Integración

### 1. Flujo Completo de Asignación y Revocación

1. Obtener la lista de roles disponibles
2. Seleccionar un rol para las pruebas
3. Obtener la lista de módulos disponibles
4. Seleccionar un módulo para las pruebas
5. Verificar que el módulo no está asignado al rol (usando el endpoint `GET /api/Modules/byRole/{roleId}`)
6. Asignar el módulo al rol (usando el endpoint `POST /api/Modules/assign-to-role`)
7. Verificar que el módulo ahora está asignado al rol
8. Revocar el módulo del rol (usando el endpoint `DELETE /api/Modules/revoke-from-role/{roleId}/{moduleId}`)
9. Verificar que el módulo ya no está asignado al rol

### 2. Prueba de Autorización

#### Caso de Prueba: Acceso sin autenticación

**Solicitud**:
```
GET /api/Modules/byRole/{roleId}
```

**Resultado esperado**:
- Código de estado: 401 Unauthorized

#### Caso de Prueba: Acceso con rol no autorizado

**Solicitud**:
```
GET /api/Modules/byRole/{roleId}
Authorization: Bearer {token_no_admin}
```

**Resultado esperado**:
- Código de estado: 403 Forbidden

## Pruebas Automatizadas

### Pruebas Unitarias

Se recomienda implementar pruebas unitarias para los siguientes componentes:

1. **ModuleRepository**:
   - `GetModulesByRoleAsync`
   - `RoleHasModuleAccessAsync`
   - `AssignModuleToRoleAsync`
   - `RevokeModuleFromRoleAsync`

2. **ModulesController**:
   - `GetModulesByRole`
   - `AssignModuleToRole`
   - `RevokeModuleFromRole`

### Ejemplo de Prueba Unitaria para ModuleRepository

```csharp
[Fact]
public async Task GetModulesByRoleAsync_WithValidRoleId_ReturnsModules()
{
    // Arrange
    var options = new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(databaseName: "TestDb")
        .Options;
    
    using (var context = new ApplicationDbContext(options))
    {
        // Crear datos de prueba
        var role = new Role { Id = Guid.NewGuid(), Name = "TestRole", IsActive = true };
        var module = new Module { Id = Guid.NewGuid(), Name = "TestModule", IsActive = true, IsEnabled = true };
        var permission = new Permission { Id = Guid.NewGuid(), Name = "Modules.View", IsActive = true };
        
        context.Roles.Add(role);
        context.Modules.Add(module);
        context.Permissions.Add(permission);
        
        // Crear relaciones
        var permissionModule = new PermissionModule
        {
            Id = Guid.NewGuid(),
            PermissionId = permission.Id,
            ModuleId = module.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        
        var rolePermission = new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "Test"
        };
        
        context.PermissionModules.Add(permissionModule);
        context.RolePermissions.Add(rolePermission);
        
        await context.SaveChangesAsync();
        
        // Act
        var repository = new ModuleRepository(context);
        var result = await repository.GetModulesByRoleAsync(role.Id);
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(module.Id, result.First().Id);
    }
}
```

## Pruebas de Rendimiento

Para evaluar el rendimiento de la funcionalidad, se recomienda realizar las siguientes pruebas:

1. **Prueba de carga**: Asignar y revocar múltiples módulos a roles simultáneamente
2. **Prueba de estrés**: Realizar un gran número de solicitudes a los endpoints de módulos y roles
3. **Prueba de escalabilidad**: Evaluar el rendimiento con un gran número de módulos y roles en la base de datos

## Lista de Verificación

Utilice esta lista para asegurarse de que todas las pruebas se han completado correctamente:

- [ ] La tabla PermissionModules se ha creado correctamente
- [ ] El permiso Modules.View existe y está asignado al rol Admin
- [ ] El endpoint GET /api/Modules/byRole/{roleId} devuelve los módulos correctos
- [ ] El endpoint POST /api/Modules/assign-to-role asigna correctamente un módulo a un rol
- [ ] El endpoint DELETE /api/Modules/revoke-from-role/{roleId}/{moduleId} revoca correctamente un módulo de un rol
- [ ] Las validaciones de seguridad funcionan correctamente
- [ ] Las pruebas unitarias pasan correctamente
- [ ] Las pruebas de rendimiento son satisfactorias

## Solución de Problemas

### Problemas Comunes

1. **Error al crear la tabla PermissionModules**:
   - Verificar que el usuario de la base de datos tiene permisos suficientes
   - Comprobar que no hay conflictos con tablas existentes

2. **Error al asignar módulos a roles**:
   - Verificar que el permiso Modules.View existe
   - Comprobar que los IDs de módulos y roles son correctos

3. **Problemas de rendimiento**:
   - Verificar que los índices están correctamente creados
   - Optimizar las consultas SQL si es necesario

## Conclusión

Siguiendo esta guía de pruebas, podrá verificar que la funcionalidad de asociación de módulos a roles funciona correctamente en todos los aspectos. Si encuentra algún problema, consulte la sección de solución de problemas o contacte con el equipo de desarrollo.
