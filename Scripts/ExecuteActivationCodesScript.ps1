# Script para ejecutar el script SQL de creación de la tabla de códigos de activación

# Configuración de la conexión a la base de datos
$serverName = "localhost"
$databaseName = "AuthSystemNewDb"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$sqlScriptPath = Join-Path $scriptDir "CreateActivationCodesTable.sql"

# Verificar que el script SQL existe
if (-not (Test-Path $sqlScriptPath)) {
    Write-Error "No se encontró el script SQL en la ruta: $sqlScriptPath"
    exit 1
}

# Leer el contenido del script SQL
$sqlScript = Get-Content -Path $sqlScriptPath -Raw

# Ejecutar el script SQL
try {
    Write-Host "Conectando a la base de datos $databaseName en $serverName..."
    
    # Crear la conexión a la base de datos
    $connectionString = "Server=$serverName;Database=$databaseName;Integrated Security=True;"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Ejecutando script SQL..."
    
    # Crear y ejecutar el comando SQL
    $command = New-Object System.Data.SqlClient.SqlCommand($sqlScript, $connection)
    $command.ExecuteNonQuery() | Out-Null
    
    Write-Host "Script SQL ejecutado correctamente."
    
    # Cerrar la conexión
    $connection.Close()
    
    Write-Host "Proceso completado exitosamente."
} catch {
    Write-Error "Error al ejecutar el script SQL: $_"
    exit 1
}
