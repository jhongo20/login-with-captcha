# Usar el token JWT proporcionado
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiY2FiNDI2Mi0wMWZmLTQxMGYtOTk0OC0xNzliMWNmOTE1NGIiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6IjE4YWNjMmVkLWUwZjEtNDk1My1iM2NiLWU4ZjU4OTYyZjRmOCIsImlhdCI6MTc0NTQyMzExMCwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJwZXJtaXNzaW9uIjpbInJvdXRlcy5lZGl0Iiwicm91dGVzLnZpZXciLCJyb3V0ZXMuZGVsZXRlIiwiTW9kdWxlcy5DcmVhdGUiLCJNb2R1bGVzLkVkaXQiLCJ1c2Vycy52aWV3IiwiTW9kdWxlcy5EZWxldGUiLCJ1c2Vycy5kZWxldGUiLCJyb3V0ZXMuY3JlYXRlIiwidXNlcnMuZWRpdCIsIk1vZHVsZXMuVmlldyIsInVzZXJzLmNyZWF0ZSJdLCJleHAiOjE3NDU0MjY3MTAsImlzcyI6IkF1dGhTeXN0ZW0iLCJhdWQiOiJBdXRoU3lzdGVtQ2xpZW50In0.lw9gINGjn_lFuvtch2x6uLr-Q4c51PK_BTTQ61rmJYI"

# Obtener una ruta y un rol existentes para probar la asignación
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

# Obtener un rol existente
$roleQuery = "SELECT TOP 1 Id, Name FROM Roles WHERE IsActive = 1"
$role = Invoke-Sqlcmd -ConnectionString $connectionString -Query $roleQuery

if ($role) {
    Write-Output "`nRol seleccionado para la prueba:"
    Write-Output "ID: $($role.Id)"
    Write-Output "Nombre: $($role.Name)"
} else {
    Write-Output "No se encontraron roles activos en la base de datos."
    exit
}

# Verificar si ya existe la asignación
$checkQuery = "SELECT COUNT(*) AS Count FROM RoleRoutes WHERE RouteId = '$($route.Id)' AND RoleId = '$($role.Id)' AND IsActive = 1"
$existingAssignment = Invoke-Sqlcmd -ConnectionString $connectionString -Query $checkQuery

if ($existingAssignment.Count -gt 0) {
    Write-Output "`nLa ruta ya está asignada al rol. Eliminando la asignación existente para probar nuevamente..."
    $deleteQuery = "DELETE FROM RoleRoutes WHERE RouteId = '$($route.Id)' AND RoleId = '$($role.Id)'"
    Invoke-Sqlcmd -ConnectionString $connectionString -Query $deleteQuery
    Write-Output "Asignación eliminada."
}

# Crear el JSON para la solicitud
$requestJson = @{
    routeId = $route.Id
    roleId = $role.Id
} | ConvertTo-Json

Write-Output "`nJSON para la solicitud de asignación:"
Write-Output $requestJson

# Configurar los headers con el token de autenticación
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Realizar la solicitud para asignar la ruta al rol
Write-Output "`nRealizando solicitud para asignar la ruta al rol..."
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes/assign" -Method Post -Body $requestJson -ContentType "application/json" -Headers $headers
    Write-Output "Respuesta: $($response.StatusCode)"
    Write-Output "Contenido: $($response.Content)"
    
    # Verificar en la base de datos que la asignación se haya creado
    $verifyQuery = "SELECT * FROM RoleRoutes WHERE RouteId = '$($route.Id)' AND RoleId = '$($role.Id)' AND IsActive = 1"
    $verification = Invoke-Sqlcmd -ConnectionString $connectionString -Query $verifyQuery
    
    if ($verification) {
        Write-Output "`nVerificación en la base de datos:"
        Write-Output "ID: $($verification.Id)"
        Write-Output "RouteId: $($verification.RouteId)"
        Write-Output "RoleId: $($verification.RoleId)"
        Write-Output "IsActive: $($verification.IsActive)"
        Write-Output "CreatedBy: $($verification.CreatedBy)"
        Write-Output "CreatedAt: $($verification.CreatedAt)"
    } else {
        Write-Output "`nNo se encontró la asignación en la base de datos a pesar de la respuesta exitosa."
    }
} catch {
    Write-Output "Error al asignar la ruta al rol: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
}
