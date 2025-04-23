$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiY2FiNDI2Mi0wMWZmLTQxMGYtOTk0OC0xNzliMWNmOTE1NGIiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6IjIxMTI2ZGNmLTFiMGItNGRkZC04NGQyLWM4NzJjMzE5YWQzYiIsImlhdCI6MTc0NTM3MDkzMywiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJwZXJtaXNzaW9uIjpbInJvdXRlcy5lZGl0Iiwicm91dGVzLnZpZXciLCJyb3V0ZXMuZGVsZXRlIiwiTW9kdWxlcy5DcmVhdGUiLCJNb2R1bGVzLkVkaXQiLCJ1c2Vycy52aWV3IiwiTW9kdWxlcy5EZWxldGUiLCJ1c2Vycy5kZWxldGUiLCJyb3V0ZXMuY3JlYXRlIiwidXNlcnMuZWRpdCIsIk1vZHVsZXMuVmlldyIsInVzZXJzLmNyZWF0ZSJdLCJleHAiOjE3NDUzNzQ1MzMsImlzcyI6IkF1dGhTeXN0ZW0iLCJhdWQiOiJBdXRoU3lzdGVtQ2xpZW50In0.ikZfhR7BYjN1FKsxSM8TCmwpgAPUDor1vwoZwo8ZGsA"

$headers = @{
    Authorization = "Bearer $token"
}

# 1. Obtener todos los módulos para usar un ID válido
Write-Output "Obteniendo módulos..."
try {
    $modulesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Modules" -Method Get -Headers $headers
    $modules = $modulesResponse.Content | ConvertFrom-Json
    
    if ($modules.Count -gt 0) {
        $moduleId = $modules[0].id
        Write-Output "Módulo seleccionado: $moduleId"
    } else {
        Write-Output "No se encontraron módulos. Creando uno nuevo..."
        
        $newModuleJson = @{
            name = "Módulo de Prueba"
            description = "Módulo creado para pruebas de rutas"
            route = "/test-module"
            icon = "test-icon"
            displayOrder = 1
            parentId = $null
            isEnabled = $true
        } | ConvertTo-Json
        
        $moduleResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Modules" -Method Post -Headers $headers -Body $newModuleJson -ContentType "application/json"
        $newModule = $moduleResponse.Content | ConvertFrom-Json
        $moduleId = $newModule.id
        Write-Output "Nuevo módulo creado con ID: $moduleId"
    }
} catch {
    Write-Output "Error al obtener/crear módulos: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta: $responseBody"
    }
    exit
}

# 2. Obtener todas las rutas
Write-Output "`nObteniendo todas las rutas..."
try {
    $routesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Get -Headers $headers
    $routes = $routesResponse.Content | ConvertFrom-Json
    Write-Output "Rutas encontradas: $($routes.Count)"
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
}

# 3. Crear una nueva ruta
Write-Output "`nCreando una nueva ruta..."
$newRouteJson = @{
    name = "Ruta de Prueba $(Get-Random)"
    description = "Ruta creada para pruebas"
    path = "/api/test/$(Get-Random)"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = $true
    isEnabled = $true
    moduleId = $moduleId
} | ConvertTo-Json

try {
    $createResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Post -Headers $headers -Body $newRouteJson -ContentType "application/json"
    $newRoute = $createResponse.Content | ConvertFrom-Json
    Write-Output "Nueva ruta creada con ID: $($newRoute.id)"
    Write-Output "Detalles: $($newRoute | ConvertTo-Json -Depth 3)"
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

# 4. Obtener rutas nuevamente para verificar que la nueva ruta se creó correctamente
Write-Output "`nVerificando rutas después de crear una nueva..."
try {
    $routesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Get -Headers $headers
    $routes = $routesResponse.Content | ConvertFrom-Json
    Write-Output "Rutas encontradas: $($routes.Count)"
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
}
