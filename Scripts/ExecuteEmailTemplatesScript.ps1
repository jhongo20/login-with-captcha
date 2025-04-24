# Script PowerShell para ejecutar el script SQL de creación de la tabla EmailTemplates

# Configuración de la conexión a la base de datos
$serverName = "localhost"
$databaseName = "AuthSystemNewDb"
$scriptPath = ".\CreateEmailTemplatesTable.sql"

# Verificar que el script SQL existe
if (-not (Test-Path $scriptPath)) {
    Write-Error "El script SQL no existe en la ruta especificada: $scriptPath"
    exit 1
}

# Mensaje de inicio
Write-Host "Ejecutando script SQL para crear la tabla EmailTemplates en la base de datos $databaseName..."

try {
    # Ejecutar el script SQL usando sqlcmd
    $command = "sqlcmd -S $serverName -d $databaseName -i $scriptPath"
    Write-Host "Ejecutando comando: $command"
    
    Invoke-Expression $command
    
    # Verificar si el comando se ejecutó correctamente
    if ($LASTEXITCODE -eq 0) {
        # Mensaje de éxito
        Write-Host "¡Script SQL ejecutado con éxito!" -ForegroundColor Green
        Write-Host "La tabla EmailTemplates ha sido creada y sembrada con datos iniciales."
    } else {
        # Mensaje de error
        Write-Host "Error al ejecutar el script SQL. Código de salida: $LASTEXITCODE" -ForegroundColor Red
        exit 1
    }
}
catch {
    # Mensaje de error
    Write-Host "Error al ejecutar el script SQL:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
