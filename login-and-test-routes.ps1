# Paso 1: Iniciar sesión para obtener un token
$loginData = @{
    username = "admin"
    password = "Admin123!"
} | ConvertTo-Json

Write-Output "Iniciando sesión para obtener token..."
try {
    $loginResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Auth/login" -Method Post -Body $loginData -ContentType "application/json"
    $loginResult = $loginResponse.Content | ConvertFrom-Json
    $token = $loginResult.token
    
    Write-Output "Login exitoso. Token obtenido."
    
    # Configurar los headers con el token
    $headers = @{
        Authorization = "Bearer $token"
    }
    
    # Paso 2: Obtener todas las rutas para verificar que funciona
    Write-Output "`nObteniendo todas las rutas..."
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
    
    # Paso 3: Crear una nueva ruta
    $newRouteJson = @{
        name = "Obtener Usuarios $(Get-Random)"
        description = "Endpoint para obtener la lista de usuarios del sistema"
        path = "/api/users/$(Get-Random)"
        httpMethod = "GET"
        displayOrder = 1
        requiresAuth = $true
        isEnabled = $true
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
    
    # Paso 4: Verificar que la ruta se haya creado correctamente
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
} catch {
    Write-Output "Error al iniciar sesión: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Output "Respuesta: $responseBody"
    }
}
