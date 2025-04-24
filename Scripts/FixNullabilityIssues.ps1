# Script para solucionar problemas de nulabilidad en el proyecto

# Deshabilitar la nulabilidad de referencia en el proyecto
Write-Host "Deshabilitando la nulabilidad de referencia en los proyectos..." -ForegroundColor Cyan

# Función para deshabilitar la nulabilidad en un archivo .csproj
function Disable-Nullability {
    param (
        [string]$projectFile
    )
    
    if (Test-Path $projectFile) {
        $content = Get-Content $projectFile -Raw
        
        # Verificar si ya existe la propiedad Nullable
        if ($content -match '<Nullable>enable</Nullable>') {
            # Reemplazar enable por disable
            $content = $content -replace '<Nullable>enable</Nullable>', '<Nullable>disable</Nullable>'
            Set-Content -Path $projectFile -Value $content
            Write-Host "  - Nulabilidad deshabilitada en $projectFile" -ForegroundColor Green
        }
        elseif ($content -match '<PropertyGroup>') {
            # Agregar la propiedad Nullable si no existe
            $content = $content -replace '<PropertyGroup>', '<PropertyGroup>`n    <Nullable>disable</Nullable>'
            Set-Content -Path $projectFile -Value $content
            Write-Host "  - Nulabilidad deshabilitada en $projectFile" -ForegroundColor Green
        }
        else {
            Write-Host "  - No se pudo modificar $projectFile" -ForegroundColor Red
        }
    }
    else {
        Write-Host "  - El archivo $projectFile no existe" -ForegroundColor Red
    }
}

# Deshabilitar la nulabilidad en todos los proyectos
$projects = @(
    "AuthSystem.Domain\AuthSystem.Domain.csproj",
    "AuthSystem.Application\AuthSystem.Application.csproj",
    "AuthSystem.Infrastructure\AuthSystem.Infrastructure.csproj",
    "AuthSystem.API\AuthSystem.API.csproj"
)

foreach ($project in $projects) {
    $projectPath = Join-Path -Path "D:\Users\Jhon\Documentos\Mintrabajo\Modulo General usuarios\AuthSystemNew" -ChildPath $project
    Disable-Nullability -projectFile $projectPath
}

Write-Host "Proceso completado." -ForegroundColor Cyan
