$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiY2FiNDI2Mi0wMWZmLTQxMGYtOTk0OC0xNzliMWNmOTE1NGIiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6ImVmYThhNjgzLWMzMDUtNDM3Ni05OTdhLTMzODQ0YjBiMzk2YiIsImlhdCI6MTc0NTQxODk5MSwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJwZXJtaXNzaW9uIjpbInJvdXRlcy5lZGl0Iiwicm91dGVzLnZpZXciLCJyb3V0ZXMuZGVsZXRlIiwiTW9kdWxlcy5DcmVhdGUiLCJNb2R1bGVzLkVkaXQiLCJ1c2Vycy52aWV3IiwiTW9kdWxlcy5EZWxldGUiLCJ1c2Vycy5kZWxldGUiLCJyb3V0ZXMuY3JlYXRlIiwidXNlcnMuZWRpdCIsIk1vZHVsZXMuVmlldyIsInVzZXJzLmNyZWF0ZSJdLCJleHAiOjE3NDU0MjI1OTEsImlzcyI6IkF1dGhTeXN0ZW0iLCJhdWQiOiJBdXRoU3lzdGVtQ2xpZW50In0.--FBPSCRLNCt6kHTELwInqywkxya8JRiaFC20Ftz8Zo"

$headers = @{
    Authorization = "Bearer $token"
}

# Paso 1: Obtener un módulo existente para usar su ID
Write-Output "Obteniendo módulos existentes..."
try {
    $modulesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Modules" -Method Get -Headers $headers
    Write-Output "Respuesta: $($modulesResponse.StatusCode)"
    $modules = $modulesResponse.Content | ConvertFrom-Json
    Write-Output "Módulos encontrados: $($modules.Count)"
    
    if ($modules.Count -gt 0) {
        $moduleId = $modules[0].id
        Write-Output "Usando módulo: $($modules[0].name) con ID: $moduleId"
    } else {
        Write-Output "No se encontraron módulos. Creando uno nuevo..."
        $newModuleJson = @{
            name = "Módulo de Prueba $(Get-Random)"
            description = "Módulo creado para pruebas"
            route = "/test"
            icon = "test-icon"
            displayOrder = 1
            isEnabled = $true
        } | ConvertTo-Json

        $createModuleResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Modules" -Method Post -Headers $headers -Body $newModuleJson -ContentType "application/json"
        $newModule = $createModuleResponse.Content | ConvertFrom-Json
        $moduleId = $newModule.id
        Write-Output "Módulo creado con ID: $moduleId"
    }
} catch {
    Write-Output "Error al obtener módulos: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta: $responseBody"
    }
    exit
}

# Paso 2: Crear una nueva ruta con todos los campos explícitos
$randomId = Get-Random
$newRouteJson = @{
    name = "Ruta de Prueba $randomId"
    description = "Ruta creada para pruebas"
    path = "/api/test/$randomId"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = $true
    isEnabled = $true
    isActive = $true
    moduleId = $moduleId
} | ConvertTo-Json

Write-Output "`nJSON de la ruta a crear:"
Write-Output $newRouteJson

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
