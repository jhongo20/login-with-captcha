$token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJiY2FiNDI2Mi0wMWZmLTQxMGYtOTk0OC0xNzliMWNmOTE1NGIiLCJ1bmlxdWVfbmFtZSI6ImFkbWluIiwiZW1haWwiOiJhZG1pbkBleGFtcGxlLmNvbSIsImp0aSI6ImVmYThhNjgzLWMzMDUtNDM3Ni05OTdhLTMzODQ0YjBiMzk2YiIsImlhdCI6MTc0NTQxODk5MSwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJwZXJtaXNzaW9uIjpbInJvdXRlcy5lZGl0Iiwicm91dGVzLnZpZXciLCJyb3V0ZXMuZGVsZXRlIiwiTW9kdWxlcy5DcmVhdGUiLCJNb2R1bGVzLkVkaXQiLCJ1c2Vycy52aWV3IiwiTW9kdWxlcy5EZWxldGUiLCJ1c2Vycy5kZWxldGUiLCJyb3V0ZXMuY3JlYXRlIiwidXNlcnMuZWRpdCIsIk1vZHVsZXMuVmlldyIsInVzZXJzLmNyZWF0ZSJdLCJleHAiOjE3NDU0MjI1OTEsImlzcyI6IkF1dGhTeXN0ZW0iLCJhdWQiOiJBdXRoU3lzdGVtQ2xpZW50In0.--FBPSCRLNCt6kHTELwInqywkxya8JRiaFC20Ftz8Zo"

$headers = @{
    Authorization = "Bearer $token"
}

# Usar un ID de módulo conocido
$moduleId = "70d4253b-8b9f-4c90-871b-98c4073050fd"

# Crear una ruta simple con todos los campos necesarios
$randomId = Get-Random
$newRouteJson = @{
    name = "Ruta Simple $randomId"
    description = "Ruta simple para pruebas"
    path = "/api/simple/$randomId"
    httpMethod = "GET"
    displayOrder = 1
    requiresAuth = $true
    isEnabled = $true
    isActive = $true
    moduleId = $moduleId
} | ConvertTo-Json

Write-Output "JSON de la ruta a crear:"
Write-Output $newRouteJson

Write-Output "`nCreando ruta..."
try {
    $createResponse = Invoke-WebRequest -Uri "http://localhost:5031/api/Routes" -Method Post -Headers $headers -Body $newRouteJson -ContentType "application/json"
    Write-Output "Éxito! Código de respuesta: $($createResponse.StatusCode)"
    Write-Output "Contenido de la respuesta: $($createResponse.Content)"
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
