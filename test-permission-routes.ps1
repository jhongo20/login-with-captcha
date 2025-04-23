# Usar el token JWT proporcionado
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiY2FiNDI2Mi0wMWZmLTQxMGYtOTk0OC0xNzliMWNmOTE1NGIiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6IjE4YWNjMmVkLWUwZjEtNDk1My1iM2NiLWU4ZjU4OTYyZjRmOCIsImlhdCI6MTc0NTQyMzExMCwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJwZXJtaXNzaW9uIjpbInJvdXRlcy5lZGl0Iiwicm91dGVzLnZpZXciLCJyb3V0ZXMuZGVsZXRlIiwiTW9kdWxlcy5DcmVhdGUiLCJNb2R1bGVzLkVkaXQiLCJ1c2Vycy52aWV3IiwiTW9kdWxlcy5EZWxldGUiLCJ1c2Vycy5kZWxldGUiLCJyb3V0ZXMuY3JlYXRlIiwidXNlcnMuZWRpdCIsIk1vZHVsZXMuVmlldyIsInVzZXJzLmNyZWF0ZSJdLCJleHAiOjE3NDU0MjY3MTAsImlzcyI6IkF1dGhTeXN0ZW0iLCJhdWQiOiJBdXRoU3lzdGVtQ2xpZW50In0.lw9gINGjn_lFuvtch2x6uLr-Q4c51PK_BTTQ61rmJYI"

# Configurar los headers con el token de autenticación
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Obtener una ruta y un permiso existentes para probar
$connectionString = "Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;"

# Obtener una ruta existente
$routeQuery = "SELECT TOP 1 Id, Name, Path FROM Routes WHERE IsActive = 1"
$route = Invoke-Sqlcmd -ConnectionString $connectionString -Query $routeQuery

if ($route) {
    Write-Output "Ruta seleccionada para la prueba:"
    Write-Output "ID: $($route.Id)"
    Write-Output "Nombre: $($route.Name)"
    Write-Output "Ruta: $($route.Path)"
} else {
    Write-Output "No se encontraron rutas activas en la base de datos."
    exit
}

# Obtener un permiso existente
$permissionQuery = "SELECT TOP 1 Id, Name, Description FROM Permissions WHERE IsActive = 1"
$permission = Invoke-Sqlcmd -ConnectionString $connectionString -Query $permissionQuery

if ($permission) {
    Write-Output "`nPermiso seleccionado para la prueba:"
    Write-Output "ID: $($permission.Id)"
    Write-Output "Nombre: $($permission.Name)"
    Write-Output "Descripción: $($permission.Description)"
} else {
    Write-Output "No se encontraron permisos activos en la base de datos."
    exit
}

# Verificar si ya existe la asignación
$checkQuery = "SELECT COUNT(*) AS Count FROM PermissionRoutes WHERE RouteId = '$($route.Id)' AND PermissionId = '$($permission.Id)' AND IsActive = 1"
$existingAssignment = Invoke-Sqlcmd -ConnectionString $connectionString -Query $checkQuery

if ($existingAssignment.Count -gt 0) {
    Write-Output "`nEl permiso ya está asignado a la ruta. Eliminando la asignación existente para probar nuevamente..."
    $deleteQuery = "UPDATE PermissionRoutes SET IsActive = 0 WHERE RouteId = '$($route.Id)' AND PermissionId = '$($permission.Id)'"
    Invoke-Sqlcmd -ConnectionString $connectionString -Query $deleteQuery
    Write-Output "Asignación desactivada."
}

# Prueba 1: Obtener permisos por ruta
Write-Output "`n1. Obteniendo permisos por ruta..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/by-route/$($route.Id)" -Method Get -Headers $headers
    Write-Output "Respuesta: $($response.StatusCode)"
    $permisos = $response.Content | ConvertFrom-Json
    Write-Output "Permisos encontrados: $($permisos.Count)"
    if ($permisos.Count -gt 0) {
        Write-Output "Primer permiso: $($permisos[0] | ConvertTo-Json -Depth 1)"
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

# Prueba 2: Obtener rutas por permiso
Write-Output "`n2. Obteniendo rutas por permiso..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/by-permission/$($permission.Id)" -Method Get -Headers $headers
    Write-Output "Respuesta: $($response.StatusCode)"
    $rutas = $response.Content | ConvertFrom-Json
    Write-Output "Rutas encontradas: $($rutas.Count)"
    if ($rutas.Count -gt 0) {
        Write-Output "Primera ruta: $($rutas[0] | ConvertTo-Json -Depth 1)"
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

# Prueba 3: Asignar permiso a ruta
Write-Output "`n3. Asignando permiso a ruta..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/assign/$($route.Id)/$($permission.Id)" -Method Post -Headers $headers
    Write-Output "Respuesta: $($response.StatusCode)"
    Write-Output "Contenido: $($response.Content)"
    
    # Verificar en la base de datos que la asignación se haya creado
    $verifyQuery = "SELECT * FROM PermissionRoutes WHERE RouteId = '$($route.Id)' AND PermissionId = '$($permission.Id)' AND IsActive = 1"
    $verification = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyQuery
    
    if ($verification) {
        Write-Output "`nVerificación en la base de datos:"
        Write-Output "ID: $($verification.Id)"
        Write-Output "RouteId: $($verification.RouteId)"
        Write-Output "PermissionId: $($verification.PermissionId)"
        Write-Output "IsActive: $($verification.IsActive)"
        Write-Output "CreatedBy: $($verification.CreatedBy)"
        Write-Output "CreatedAt: $($verification.CreatedAt)"
    } else {
        Write-Output "`nNo se encontró la asignación en la base de datos a pesar de la respuesta exitosa."
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

# Prueba 4: Revocar permiso de ruta
Write-Output "`n4. Revocando permiso de ruta..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/PermissionRoutes/revoke/$($route.Id)/$($permission.Id)" -Method Delete -Headers $headers
    Write-Output "Respuesta: $($response.StatusCode)"
    Write-Output "Contenido: $($response.Content)"
    
    # Verificar en la base de datos que la asignación se haya desactivado
    $verifyQuery = "SELECT * FROM PermissionRoutes WHERE RouteId = '$($route.Id)' AND PermissionId = '$($permission.Id)'"
    $verification = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyQuery
    
    if ($verification) {
        Write-Output "`nVerificación en la base de datos:"
        Write-Output "ID: $($verification.Id)"
        Write-Output "RouteId: $($verification.RouteId)"
        Write-Output "PermissionId: $($verification.PermissionId)"
        Write-Output "IsActive: $($verification.IsActive)"
        Write-Output "LastModifiedBy: $($verification.LastModifiedBy)"
        Write-Output "LastModifiedAt: $($verification.LastModifiedAt)"
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
