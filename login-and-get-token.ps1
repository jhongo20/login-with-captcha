$loginData = @{
    username = "admin"
    password = "Admin123!"
} | ConvertTo-Json

Write-Output "Iniciando sesión para obtener token..."
try {
    $loginResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Auth/login" -Method Post -Body $loginData -ContentType "application/json"
    Write-Output "Respuesta: $($loginResponse.StatusCode)"
    
    $tokenData = $loginResponse.Content | ConvertFrom-Json
    $token = $tokenData.token
    
    Write-Output "Token obtenido: $token"
    
    # Guardar el token en un archivo para uso futuro
    $token | Out-File -FilePath ".\token.txt"
    Write-Output "Token guardado en token.txt"
    
    # Crear un script de prueba con el token actualizado
    $testScript = @"
`$token = "$token"

`$headers = @{
    Authorization = "Bearer `$token"
}

# Usar un ID de módulo conocido
`$moduleId = "70d4253b-8b9f-4c90-871b-98c4073050fd"

# Crear una ruta simple con todos los campos necesarios
`$randomId = Get-Random
`$newRouteJson = @{
    name = "Ruta Simple `$randomId"
    description = "Ruta simple para pruebas"
    path = "/api/simple/`$randomId"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = `$true
    isEnabled = `$true
    isActive = `$true
    moduleId = `$moduleId
} | ConvertTo-Json

Write-Output "JSON de la ruta a crear:"
Write-Output `$newRouteJson

Write-Output "`nCreando ruta..."
try {
    `$createResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Post -Headers `$headers -Body `$newRouteJson -ContentType "application/json"
    Write-Output "Éxito! Código de respuesta: `$(`$createResponse.StatusCode)"
    Write-Output "Contenido de la respuesta: `$(`$createResponse.Content)"
} catch {
    Write-Output "Error al crear ruta: `$(`$_.Exception.Message)"
    if (`$_.Exception.Response) {
        `$reader = New-Object System.IO.StreamReader(`$_.Exception.Response.GetResponseStream())
        `$reader.BaseStream.Position = 0
        `$reader.DiscardBufferedData()
        `$responseBody = `$reader.ReadToEnd()
        Write-Output "Respuesta de error: `$responseBody"
    }
}
"@
    
    $testScript | Out-File -FilePath ".\test-route-with-new-token.ps1"
    Write-Output "Script de prueba creado en test-route-with-new-token.ps1"
    
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
