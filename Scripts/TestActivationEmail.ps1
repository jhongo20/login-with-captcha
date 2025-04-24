# Script para probar el envío de correos de activación

# Token de autenticación (reemplazar con un token válido)
$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiY2FiNDI2Mi0wMWZmLTQxMGYtOTk0OC0xNzliMWNmOTE1NGIiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6ImZmODdkOWRhLTE3ODktNDVkOC1iMjllLTFmNWUzNjEwOTYyMSIsImlhdCI6MTc0NTQzOTcwNywiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJwZXJtaXNzaW9uIjpbInJvdXRlcy5lZGl0Iiwicm91dGVzLnZpZXciLCJyb3V0ZXMuZGVsZXRlIiwiTW9kdWxlcy5DcmVhdGUiLCJNb2R1bGVzLkVkaXQiLCJ1c2Vycy52aWV3IiwiTW9kdWxlcy5EZWxldGUiLCJ1c2Vycy5kZWxldGUiLCJyb3V0ZXMuY3JlYXRlIiwidXNlcnMuZWRpdCIsIk1vZHVsZXMuVmlldyIsInVzZXJzLmNyZWF0ZSJdLCJleHAiOjE3NDU0NDMzMDcsImlzcyI6IkF1dGhTeXN0ZW0iLCJhdWQiOiJBdXRoU3lzdGVtQ2xpZW50In0.b-9iU3b-pHIaxBVHWV5fz8e7dh9laZOy12HvHPNzVTE"

# Configuración de la solicitud
$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# Generar un código de activación aleatorio
function GenerateActivationCode {
    $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    $code = ""
    $random = New-Object System.Random
    
    for ($i = 0; $i -lt 6; $i++) {
        $code += $chars[$random.Next(0, $chars.Length)]
    }
    
    return $code
}

# Generar un código de activación
$activationCode = GenerateActivationCode

# Datos para el correo electrónico de activación
$emailData = @{
    templateName = "ActivationCode"
    email = "jhongopruebas@gmail.com"  # Reemplazar con el correo del usuario
    templateData = @{
        FullName = "Usuario de Prueba"
        Username = "usuarioprueba"
        Email = "jhongopruebas@gmail.com"  # Reemplazar con el correo del usuario
        ActivationCode = $activationCode
        ExpirationTime = "24 horas"
    }
    attachments = @()
}

# Convertir a JSON
$jsonBody = $emailData | ConvertTo-Json -Depth 5

Write-Host "Enviando solicitud a http://localhost:5031/api/Email/send..."
Write-Host "Código de activación generado: $activationCode"
Write-Host "Cuerpo de la solicitud:"
Write-Host $jsonBody

# Realizar la solicitud
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5031/api/Email/send" -Method Post -Headers $headers -Body $jsonBody -ErrorAction Stop
    
    Write-Host "Respuesta recibida:"
    Write-Host "Código de estado: $($response.StatusCode)"
    Write-Host "Contenido: $($response.Content)"
    Write-Host "Correo de activación enviado correctamente."
} catch {
    Write-Host "Error al enviar el correo de activación:"
    Write-Host "Código de estado: $($_.Exception.Response.StatusCode.value__)"
    
    # Intentar obtener el cuerpo de la respuesta de error
    try {
        $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
        $reader.BaseStream.Position = 0
        $reader.DiscardBufferedData()
        $responseBody = $reader.ReadToEnd()
        Write-Host "Detalle del error: $responseBody"
    } catch {
        Write-Host "No se pudo obtener el detalle del error."
    }
}

Write-Host "Presiona cualquier tecla para salir..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
