$connectionString = "Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;"

# Crear una nueva ruta directamente usando ADO.NET
$routeId = [System.Guid]::NewGuid().ToString()
$routeName = "Ruta ADO.NET Test " + (Get-Random)
$routePath = "/api/adonet/test/" + (Get-Random)
$moduleId = "70d4253b-8b9f-4c90-871b-98c4073050fd" # ID del módulo conocido
$createdAt = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

$insertQuery = @"
INSERT INTO Routes (
    Id, 
    Name, 
    Description, 
    Path, 
    HttpMethod, 
    DisplayOrder, 
    RequiresAuth, 
    IsEnabled, 
    IsActive, 
    ModuleId, 
    CreatedAt, 
    CreatedBy, 
    LastModifiedAt, 
    LastModifiedBy
)
VALUES (
    '$routeId', 
    '$routeName', 
    'Ruta creada directamente con ADO.NET para pruebas', 
    '$routePath', 
    'GET', 
    1, 
    1, -- RequiresAuth = true
    1, -- IsEnabled = true
    1, -- IsActive = true
    '$moduleId', 
    '$createdAt', 
    'System', 
    '$createdAt', 
    'System'
);
"@

Write-Output "Insertando ruta directamente en la base de datos..."
Write-Output "Query: $insertQuery"

try {
    # Usar Invoke-Sqlcmd si está disponible
    if (Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue) {
        Invoke-Sqlcmd -Query $insertQuery -ConnectionString $connectionString -Verbose
        Write-Output "Ruta insertada correctamente con Invoke-Sqlcmd."
        
        # Verificar que la ruta se haya creado correctamente
        $selectQuery = "SELECT * FROM Routes WHERE Id = '$routeId'"
        $route = Invoke-Sqlcmd -Query $selectQuery -ConnectionString $connectionString
        Write-Output "Ruta creada:"
        $route | Format-Table -AutoSize
    }
    # Si no, intentar con sqlcmd.exe
    else {
        $tempFile = [System.IO.Path]::GetTempFileName()
        $insertQuery | Out-File -FilePath $tempFile -Encoding utf8
        
        $result = sqlcmd -S localhost -d AuthSystemNewDb -i $tempFile -o "sql-insert-output.txt"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Output "Ruta insertada correctamente con sqlcmd.exe."
            if (Test-Path "sql-insert-output.txt") {
                Write-Output "Resultado de la inserción:"
                Get-Content "sql-insert-output.txt"
            }
            
            # Verificar que la ruta se haya creado correctamente
            $selectFile = [System.IO.Path]::GetTempFileName()
            "SELECT * FROM Routes WHERE Id = '$routeId'" | Out-File -FilePath $selectFile -Encoding utf8
            
            sqlcmd -S localhost -d AuthSystemNewDb -i $selectFile -o "sql-select-output.txt"
            
            if (Test-Path "sql-select-output.txt") {
                Write-Output "Ruta creada:"
                Get-Content "sql-select-output.txt"
            }
            
            Remove-Item $selectFile -Force
        }
        else {
            Write-Output "Error al insertar la ruta: $LASTEXITCODE"
        }
        
        # Limpiar archivo temporal
        Remove-Item $tempFile -Force
    }
}
catch {
    Write-Output "Error al insertar la ruta: $($_.Exception.Message)"
}
