# Primero, obtener las rutas existentes para verificar que la API está funcionando
Write-Output "Obteniendo rutas existentes..."
try {
    $routesResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Get
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
}

# Crear una nueva ruta usando el endpoint API
$moduleId = "70d4253b-8b9f-4c90-871b-98c4073050fd" # ID del módulo conocido
$randomId = Get-Random
$newRouteJson = @{
    name = "Ruta API Test $randomId"
    description = "Ruta creada a través de la API para pruebas"
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

Write-Output "`nCreando ruta a través de la API..."
try {
    $createResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Post -Body $newRouteJson -ContentType "application/json"
    Write-Output "Éxito! Código de respuesta: $($createResponse.StatusCode)"
    Write-Output "Contenido de la respuesta: $($createResponse.Content)"
    
    # Verificar que la ruta se haya creado correctamente
    $routeData = $createResponse.Content | ConvertFrom-Json
    $routeId = $routeData.id
    
    Write-Output "`nVerificando que la ruta se haya creado correctamente..."
    $checkResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes/$routeId" -Method Get
    $createdRoute = $checkResponse.Content | ConvertFrom-Json
    Write-Output "Ruta creada: $($createdRoute | ConvertTo-Json -Depth 3)"
} catch {
    Write-Output "Error al crear ruta: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta de error: $responseBody"
    }
}
