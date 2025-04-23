$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiY2FiNDI2Mi0wMWZmLTQxMGYtOTk0OC0xNzliMWNmOTE1NGIiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6ImVmYThhNjgzLWMzMDUtNDM3Ni05OTdhLTMzODQ0YjBiMzk2YiIsImlhdCI6MTc0NTQxODk5MSwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJwZXJtaXNzaW9uIjpbInJvdXRlcy5lZGl0Iiwicm91dGVzLnZpZXciLCJyb3V0ZXMuZGVsZXRlIiwiTW9kdWxlcy5DcmVhdGUiLCJNb2R1bGVzLkVkaXQiLCJ1c2Vycy52aWV3IiwiTW9kdWxlcy5EZWxldGUiLCJ1c2Vycy5kZWxldGUiLCJyb3V0ZXMuY3JlYXRlIiwidXNlcnMuZWRpdCIsIk1vZHVsZXMuVmlldyIsInVzZXJzLmNyZWF0ZSJdLCJleHAiOjE3NDU0MjI1OTEsImlzcyI6IkF1dGhTeXN0ZW0iLCJhdWQiOiJBdXRoU3lzdGVtQ2xpZW50In0.--FBPSCRLNCt6kHTELwInqywkxya8JRiaFC20Ftz8Zo"

$headers = @{
    Authorization = "Bearer $token"
}

# Paso 1: Obtener todas las rutas para verificar que funciona
Write-Output "Obteniendo todas las rutas..."
try {
    $routesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Get -Headers $headers
    Write-Output "Respuesta: $($routesResponse.StatusCode)"
    $routes = $routesResponse.Content | ConvertFrom-Json
    Write-Output "Rutas encontradas: $($routes.Count)"
    
    # Mostrar la primera ruta como ejemplo
    if ($routes.Count -gt 0) {
        Write-Output "Primera ruta: $($routes[0] | ConvertTo-Json -Depth 3)"
    }
} catch {
    Write-Output "Error al obtener rutas: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta: $responseBody"
    }
    exit
}

# Paso 2: Crear una nueva ruta
$newRouteJson = @{
    name = "Obtener Usuarios $(Get-Random)"
    description = "Endpoint para obtener la lista de usuarios del sistema"
    path = "/api/users/$(Get-Random)"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = $true
    isEnabled = $true
    isActive = $true
    moduleId = "70d4253b-8b9f-4c90-871b-98c4073050fd"
} | ConvertTo-Json

Write-Output "`nCreando nueva ruta..."
try {
    $createResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Post -Headers $headers -Body $newRouteJson -ContentType "application/json"
    Write-Output "Respuesta: $($createResponse.StatusCode)"
    Write-Output "Contenido: $($createResponse.Content)"
    
    # Extraer el ID de la ruta creada
    $newRoute = $createResponse.Content | ConvertFrom-Json
    $newRouteId = $newRoute.id
    Write-Output "Nueva ruta creada con ID: $newRouteId"
} catch {
    Write-Output "Error al crear ruta: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta: $responseBody"
    }
}

# Paso 3: Verificar que la ruta se haya creado correctamente
Write-Output "`nVerificando rutas después de crear una nueva..."
try {
    $routesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Get -Headers $headers
    Write-Output "Respuesta: $($routesResponse.StatusCode)"
    $routes = $routesResponse.Content | ConvertFrom-Json
    Write-Output "Rutas encontradas: $($routes.Count)"
    
    # Mostrar la última ruta creada (debería ser la que acabamos de crear)
    if ($routes.Count -gt 0) {
        $lastRoute = $routes | Sort-Object -Property createdAt -Descending | Select-Object -First 1
        Write-Output "Última ruta creada: $($lastRoute | ConvertTo-Json -Depth 3)"
    }
} catch {
    Write-Output "Error al obtener rutas: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta: $responseBody"
    }
}
