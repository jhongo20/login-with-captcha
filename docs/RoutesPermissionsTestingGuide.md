# Guía de Pruebas: Rutas, Roles y Permisos

## Introducción

Esta guía proporciona instrucciones detalladas para probar la funcionalidad del sistema de rutas, roles y permisos en AuthSystem. Está diseñada para desarrolladores, testers y administradores del sistema que necesiten verificar que la integración entre estos componentes funciona correctamente.

## Requisitos Previos

1. Instancia de AuthSystem en ejecución
2. Base de datos configurada correctamente
3. Credenciales de administrador para acceder a la API
4. Herramientas para realizar solicitudes HTTP (Postman, curl, PowerShell)

## Configuración del Entorno de Pruebas

### 1. Obtener un Token JWT

Antes de comenzar las pruebas, necesitas obtener un token JWT válido:

```powershell
$loginJson = @{
    username = "admin"
    password = "Admin123!"
} | ConvertTo-Json

$loginResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/auth/login" -Method Post -Body $loginJson -ContentType "application/json"
$token = ($loginResponse.Content | ConvertFrom-Json).token

# Configurar los headers para las solicitudes
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}
```

### 2. Verificar Módulos Existentes

Obtén una lista de los módulos disponibles para usar en las pruebas:

```powershell
$modulesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Modules" -Method Get -Headers $headers
$modules = $modulesResponse.Content | ConvertFrom-Json
$moduleId = $modules[0].id  # Usar el primer módulo como ejemplo
```

### 3. Verificar Roles Existentes

Obtén una lista de los roles disponibles para usar en las pruebas:

```powershell
$rolesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Roles" -Method Get -Headers $headers
$roles = $rolesResponse.Content | ConvertFrom-Json
$roleId = $roles[0].id  # Usar el primer rol como ejemplo
```

### 4. Verificar Permisos Existentes

Obtén una lista de los permisos disponibles para usar en las pruebas:

```powershell
$permissionsResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Permissions" -Method Get -Headers $headers
$permissions = $permissionsResponse.Content | ConvertFrom-Json
$permissionId = $permissions[0].id  # Usar el primer permiso como ejemplo
```

## Casos de Prueba

### 1. Crear una Ruta

**Objetivo**: Verificar que se puede crear una nueva ruta correctamente.

**Pasos**:
1. Generar un identificador único para la ruta de prueba
2. Enviar una solicitud POST para crear la ruta

```powershell
$randomId = Get-Random
$routeJson = @{
    name = "Ruta de Prueba $randomId"
    description = "Ruta creada para pruebas"
    path = "/api/test/$randomId"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = $true
    isEnabled = $true
    moduleId = $moduleId
} | ConvertTo-Json

$createRouteResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Post -Body $routeJson -Headers $headers
$route = $createRouteResponse.Content | ConvertFrom-Json
$routeId = $route.id
```

**Resultado esperado**: Código de estado 201 (Created) y un objeto JSON con los detalles de la ruta creada.

### 2. Asignar una Ruta a un Rol

**Objetivo**: Verificar que se puede asignar una ruta a un rol correctamente.

**Pasos**:
1. Enviar una solicitud POST para asignar la ruta al rol

```powershell
$assignRouteRoleJson = @{
    routeId = $routeId
    roleId = $roleId
} | ConvertTo-Json

$assignRouteRoleResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes/assign" -Method Post -Body $assignRouteRoleJson -Headers $headers
```

**Resultado esperado**: Código de estado 200 (OK) y un mensaje confirmando que la ruta se ha asignado correctamente al rol.

### 3. Verificar la Asignación de Ruta a Rol

**Objetivo**: Verificar que la asignación de la ruta al rol se ha guardado correctamente en la base de datos.

**Pasos**:
1. Consultar la base de datos para verificar la asignación

```powershell
$connectionString = "Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;"
$verifyQuery = "SELECT * FROM RoleRoutes WHERE RouteId = '$routeId' AND RoleId = '$roleId' AND IsActive = 1"
$verification = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyQuery

if ($verification) {
    Write-Output "Verificación exitosa: La ruta está asignada al rol"
    Write-Output "ID: $($verification.Id)"
    Write-Output "RouteId: $($verification.RouteId)"
    Write-Output "RoleId: $($verification.RoleId)"
    Write-Output "IsActive: $($verification.IsActive)"
} else {
    Write-Output "Error: La ruta no está asignada al rol"
}
```

**Resultado esperado**: Mensaje confirmando que la ruta está asignada al rol y detalles de la asignación.

### 4. Asignar un Permiso a una Ruta

**Objetivo**: Verificar que se puede asignar un permiso a una ruta correctamente.

**Pasos**:
1. Enviar una solicitud POST para asignar el permiso a la ruta

```powershell
$assignPermissionRouteResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/assign/$routeId/$permissionId" -Method Post -Headers $headers
```

**Resultado esperado**: Código de estado 200 (OK) y un mensaje confirmando que el permiso se ha asignado correctamente a la ruta.

### 5. Verificar la Asignación de Permiso a Ruta

**Objetivo**: Verificar que la asignación del permiso a la ruta se ha guardado correctamente en la base de datos.

**Pasos**:
1. Consultar la base de datos para verificar la asignación

```powershell
$verifyPermissionQuery = "SELECT * FROM PermissionRoutes WHERE RouteId = '$routeId' AND PermissionId = '$permissionId' AND IsActive = 1"
$verificationPermission = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyPermissionQuery

if ($verificationPermission) {
    Write-Output "Verificación exitosa: El permiso está asignado a la ruta"
    Write-Output "ID: $($verificationPermission.Id)"
    Write-Output "RouteId: $($verificationPermission.RouteId)"
    Write-Output "PermissionId: $($verificationPermission.PermissionId)"
    Write-Output "IsActive: $($verificationPermission.IsActive)"
} else {
    Write-Output "Error: El permiso no está asignado a la ruta"
}
```

**Resultado esperado**: Mensaje confirmando que el permiso está asignado a la ruta y detalles de la asignación.

### 6. Obtener Permisos por Ruta

**Objetivo**: Verificar que se pueden obtener todos los permisos asignados a una ruta.

**Pasos**:
1. Enviar una solicitud GET para obtener los permisos de la ruta

```powershell
$getPermissionsByRouteResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/by-route/$routeId" -Method Get -Headers $headers
$routePermissions = $getPermissionsByRouteResponse.Content | ConvertFrom-Json

Write-Output "Permisos asignados a la ruta:"
foreach ($permission in $routePermissions) {
    Write-Output "- $($permission.name): $($permission.description)"
}
```

**Resultado esperado**: Lista de permisos asignados a la ruta, incluyendo el permiso que acabamos de asignar.

### 7. Obtener Rutas por Permiso

**Objetivo**: Verificar que se pueden obtener todas las rutas asignadas a un permiso.

**Pasos**:
1. Enviar una solicitud GET para obtener las rutas del permiso

```powershell
$getRoutesByPermissionResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/by-permission/$permissionId" -Method Get -Headers $headers
$permissionRoutes = $getRoutesByPermissionResponse.Content | ConvertFrom-Json

Write-Output "Rutas asignadas al permiso:"
foreach ($route in $permissionRoutes) {
    Write-Output "- $($route.name): $($route.path) ($($route.httpMethod))"
}
```

**Resultado esperado**: Lista de rutas asignadas al permiso, incluyendo la ruta que acabamos de asignar.

### 8. Revocar un Permiso de una Ruta

**Objetivo**: Verificar que se puede revocar un permiso de una ruta correctamente.

**Pasos**:
1. Enviar una solicitud DELETE para revocar el permiso de la ruta

```powershell
$revokePermissionRouteResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/revoke/$routeId/$permissionId" -Method Delete -Headers $headers
```

**Resultado esperado**: Código de estado 200 (OK) y un mensaje confirmando que el permiso se ha revocado correctamente de la ruta.

### 9. Verificar la Revocación del Permiso

**Objetivo**: Verificar que la revocación del permiso se ha guardado correctamente en la base de datos.

**Pasos**:
1. Consultar la base de datos para verificar la revocación

```powershell
$verifyRevokeQuery = "SELECT * FROM PermissionRoutes WHERE RouteId = '$routeId' AND PermissionId = '$permissionId'"
$verificationRevoke = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyRevokeQuery

if ($verificationRevoke -and $verificationRevoke.IsActive -eq $false) {
    Write-Output "Verificación exitosa: El permiso ha sido revocado de la ruta"
    Write-Output "ID: $($verificationRevoke.Id)"
    Write-Output "IsActive: $($verificationRevoke.IsActive)"
} else {
    Write-Output "Error: El permiso no ha sido revocado correctamente"
}
```

**Resultado esperado**: Mensaje confirmando que el permiso ha sido revocado de la ruta (IsActive = false).

## Script Completo de Pruebas

A continuación se presenta un script PowerShell completo que ejecuta todas las pruebas anteriores:

```powershell
# Configuración
$baseUrl = "http://localhost:5031"
$connectionString = "Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;"

# 1. Obtener token JWT
Write-Output "1. Obteniendo token JWT..."
$loginJson = @{
    username = "admin"
    password = "Admin123!"
} | ConvertTo-Json

try {
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/api/auth/login" -Method Post -Body $loginJson -ContentType "application/json"
    $token = ($loginResponse.Content | ConvertFrom-Json).token
    $headers = @{
        "Authorization" = "Bearer $token"
        "Content-Type" = "application/json"
    }
    Write-Output "Token obtenido correctamente."
} catch {
    Write-Output "Error al obtener token: $($_.Exception.Message)"
    exit
}

# 2. Obtener módulo para las pruebas
Write-Output "`n2. Obteniendo módulo para las pruebas..."
try {
    $modulesResponse = Invoke-WebRequest -Uri "$baseUrl/api/Modules" -Method Get -Headers $headers
    $modules = $modulesResponse.Content | ConvertFrom-Json
    $moduleId = $modules[0].id
    Write-Output "Módulo seleccionado: $($modules[0].name) ($moduleId)"
} catch {
    Write-Output "Error al obtener módulos: $($_.Exception.Message)"
    exit
}

# 3. Obtener rol para las pruebas
Write-Output "`n3. Obteniendo rol para las pruebas..."
try {
    $rolesResponse = Invoke-WebRequest -Uri "$baseUrl/api/Roles" -Method Get -Headers $headers
    $roles = $rolesResponse.Content | ConvertFrom-Json
    $roleId = $roles[0].id
    Write-Output "Rol seleccionado: $($roles[0].name) ($roleId)"
} catch {
    Write-Output "Error al obtener roles: $($_.Exception.Message)"
    exit
}

# 4. Obtener permiso para las pruebas
Write-Output "`n4. Obteniendo permiso para las pruebas..."
try {
    $permissionsResponse = Invoke-WebRequest -Uri "$baseUrl/api/Permissions" -Method Get -Headers $headers
    $permissions = $permissionsResponse.Content | ConvertFrom-Json
    $permissionId = $permissions[0].id
    Write-Output "Permiso seleccionado: $($permissions[0].name) ($permissionId)"
} catch {
    Write-Output "Error al obtener permisos: $($_.Exception.Message)"
    exit
}

# 5. Crear ruta de prueba
Write-Output "`n5. Creando ruta de prueba..."
$randomId = Get-Random
$routeJson = @{
    name = "Ruta de Prueba $randomId"
    description = "Ruta creada para pruebas"
    path = "/api/test/$randomId"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = $true
    isEnabled = $true
    moduleId = $moduleId
} | ConvertTo-Json

try {
    $createRouteResponse = Invoke-WebRequest -Uri "$baseUrl/api/Routes" -Method Post -Body $routeJson -Headers $headers
    $route = $createRouteResponse.Content | ConvertFrom-Json
    $routeId = $route.id
    Write-Output "Ruta creada correctamente: $($route.name) ($routeId)"
} catch {
    Write-Output "Error al crear ruta: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
    exit
}

# 6. Asignar ruta a rol
Write-Output "`n6. Asignando ruta a rol..."
$assignRouteRoleJson = @{
    routeId = $routeId
    roleId = $roleId
} | ConvertTo-Json

try {
    $assignRouteRoleResponse = Invoke-WebRequest -Uri "$baseUrl/api/Routes/assign" -Method Post -Body $assignRouteRoleJson -Headers $headers
    Write-Output "Respuesta: $($assignRouteRoleResponse.StatusCode)"
    Write-Output "Contenido: $($assignRouteRoleResponse.Content)"
    
    # Verificar en la base de datos
    $verifyQuery = "SELECT * FROM RoleRoutes WHERE RouteId = '$routeId' AND RoleId = '$roleId' AND IsActive = 1"
    $verification = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyQuery
    
    if ($verification) {
        Write-Output "`nVerificación en la base de datos:"
        Write-Output "ID: $($verification.Id)"
        Write-Output "RouteId: $($verification.RouteId)"
        Write-Output "RoleId: $($verification.RoleId)"
        Write-Output "IsActive: $($verification.IsActive)"
    } else {
        Write-Output "`nNo se encontró la asignación en la base de datos."
    }
} catch {
    Write-Output "Error al asignar ruta a rol: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
}

# 7. Asignar permiso a ruta
Write-Output "`n7. Asignando permiso a ruta..."
try {
    $assignPermissionRouteResponse = Invoke-WebRequest -Uri "$baseUrl/api/PermissionRoutes/assign/$routeId/$permissionId" -Method Post -Headers $headers
    Write-Output "Respuesta: $($assignPermissionRouteResponse.StatusCode)"
    Write-Output "Contenido: $($assignPermissionRouteResponse.Content)"
    
    # Verificar en la base de datos
    $verifyPermissionQuery = "SELECT * FROM PermissionRoutes WHERE RouteId = '$routeId' AND PermissionId = '$permissionId' AND IsActive = 1"
    $verificationPermission = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyPermissionQuery
    
    if ($verificationPermission) {
        Write-Output "`nVerificación en la base de datos:"
        Write-Output "ID: $($verificationPermission.Id)"
        Write-Output "RouteId: $($verificationPermission.RouteId)"
        Write-Output "PermissionId: $($verificationPermission.PermissionId)"
        Write-Output "IsActive: $($verificationPermission.IsActive)"
    } else {
        Write-Output "`nNo se encontró la asignación en la base de datos."
    }
} catch {
    Write-Output "Error al asignar permiso a ruta: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
}

# 8. Obtener permisos por ruta
Write-Output "`n8. Obteniendo permisos por ruta..."
try {
    $getPermissionsByRouteResponse = Invoke-WebRequest -Uri "$baseUrl/api/PermissionRoutes/by-route/$routeId" -Method Get -Headers $headers
    $routePermissions = $getPermissionsByRouteResponse.Content | ConvertFrom-Json
    
    Write-Output "Permisos asignados a la ruta:"
    if ($routePermissions.Count -gt 0) {
        foreach ($permission in $routePermissions) {
            Write-Output "- $($permission.name): $($permission.description)"
        }
    } else {
        Write-Output "No hay permisos asignados a la ruta."
    }
} catch {
    Write-Output "Error al obtener permisos por ruta: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
}

# 9. Obtener rutas por permiso
Write-Output "`n9. Obteniendo rutas por permiso..."
try {
    $getRoutesByPermissionResponse = Invoke-WebRequest -Uri "$baseUrl/api/PermissionRoutes/by-permission/$permissionId" -Method Get -Headers $headers
    $permissionRoutes = $getRoutesByPermissionResponse.Content | ConvertFrom-Json
    
    Write-Output "Rutas asignadas al permiso:"
    if ($permissionRoutes.Count -gt 0) {
        foreach ($route in $permissionRoutes) {
            Write-Output "- $($route.name): $($route.path) ($($route.httpMethod))"
        }
    } else {
        Write-Output "No hay rutas asignadas al permiso."
    }
} catch {
    Write-Output "Error al obtener rutas por permiso: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
}

# 10. Revocar permiso de ruta
Write-Output "`n10. Revocando permiso de ruta..."
try {
    $revokePermissionRouteResponse = Invoke-WebRequest -Uri "$baseUrl/api/PermissionRoutes/revoke/$routeId/$permissionId" -Method Delete -Headers $headers
    Write-Output "Respuesta: $($revokePermissionRouteResponse.StatusCode)"
    Write-Output "Contenido: $($revokePermissionRouteResponse.Content)"
    
    # Verificar en la base de datos
    $verifyRevokeQuery = "SELECT * FROM PermissionRoutes WHERE RouteId = '$routeId' AND PermissionId = '$permissionId'"
    $verificationRevoke = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyRevokeQuery
    
    if ($verificationRevoke) {
        Write-Output "`nVerificación en la base de datos:"
        Write-Output "ID: $($verificationRevoke.Id)"
        Write-Output "RouteId: $($verificationRevoke.RouteId)"
        Write-Output "PermissionId: $($verificationRevoke.PermissionId)"
        Write-Output "IsActive: $($verificationRevoke.IsActive)"
    } else {
        Write-Output "`nNo se encontró la asignación en la base de datos."
    }
} catch {
    Write-Output "Error al revocar permiso de ruta: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
}

Write-Output "`n¡Pruebas completadas!"
```

## Interpretación de Resultados

### Prueba Exitosa

Una prueba se considera exitosa cuando:

1. La solicitud HTTP devuelve un código de estado 200 (OK) o 201 (Created)
2. La verificación en la base de datos confirma que los datos se han guardado correctamente
3. Las consultas posteriores muestran los datos esperados

### Prueba Fallida

Una prueba se considera fallida cuando:

1. La solicitud HTTP devuelve un código de error (4xx o 5xx)
2. La verificación en la base de datos no encuentra los datos esperados o los encuentra en un estado incorrecto
3. Las consultas posteriores no muestran los datos esperados

## Solución de Problemas Comunes

Si las pruebas fallan, consulta la [Guía de Solución de Problemas](./RoutesPermissionsTroubleshooting.md) para obtener información sobre cómo diagnosticar y resolver problemas comunes.

## Conclusión

Esta guía proporciona un conjunto completo de pruebas para verificar la funcionalidad del sistema de rutas, roles y permisos en AuthSystem. Al seguir estos pasos, puedes asegurarte de que la integración entre estos componentes funciona correctamente y que los usuarios tienen el acceso adecuado a las diferentes partes de la aplicación.
