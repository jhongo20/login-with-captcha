$connectionString = "Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;"
$sqlScript = Get-Content -Path ".\fix-routes-table.sql" -Raw

Write-Output "Ejecutando script SQL para corregir la tabla Routes..."

try {
    # Usar Invoke-Sqlcmd si está disponible
    if (Get-Command Invoke-Sqlcmd -ErrorAction SilentlyContinue) {
        Invoke-Sqlcmd -Query $sqlScript -ConnectionString $connectionString -Verbose
        Write-Output "Script SQL ejecutado correctamente con Invoke-Sqlcmd."
    }
    # Si no, intentar con sqlcmd.exe
    else {
        $tempFile = [System.IO.Path]::GetTempFileName()
        $sqlScript | Out-File -FilePath $tempFile -Encoding utf8
        
        $result = sqlcmd -S localhost -d AuthSystemNewDb -i $tempFile -o "sql-output.txt"
        
        if ($LASTEXITCODE -eq 0) {
            Write-Output "Script SQL ejecutado correctamente con sqlcmd.exe."
            if (Test-Path "sql-output.txt") {
                Write-Output "Resultado de la ejecución:"
                Get-Content "sql-output.txt"
            }
        }
        else {
            Write-Output "Error al ejecutar el script SQL: $LASTEXITCODE"
        }
        
        # Limpiar archivo temporal
        Remove-Item $tempFile -Force
    }
}
catch {
    Write-Output "Error al ejecutar el script SQL: $($_.Exception.Message)"
}
