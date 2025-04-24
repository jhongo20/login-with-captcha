# Script para probar la activación de usuarios
# Este script genera un código de activación para un usuario y envía un correo electrónico

# Configuración
$connectionString = "Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
$smtpServer = "smtp.gmail.com"
$smtpPort = 587
$smtpUsername = "jhongopruebas@gmail.com"
$smtpPassword = "tnoeowgsvuhfxfcb"
$senderEmail = "jhongopruebas@gmail.com"
$senderName = "AuthSystem"

# Cargar ensamblados necesarios
Add-Type -AssemblyName System.Data.SqlClient
Add-Type -AssemblyName System.Net.Mail

# Función para generar un código de activación aleatorio
function Generate-ActivationCode {
    $chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    $code = ""
    $random = New-Object System.Random
    
    for ($i = 0; $i -lt 6; $i++) {
        $code += $chars[$random.Next(0, $chars.Length)]
    }
    
    return $code
}

# Función para enviar un correo electrónico
function Send-Email {
    param (
        [string]$to,
        [string]$subject,
        [string]$body
    )
    
    try {
        $smtp = New-Object System.Net.Mail.SmtpClient($smtpServer, $smtpPort)
        $smtp.EnableSsl = $true
        $smtp.Credentials = New-Object System.Net.NetworkCredential($smtpUsername, $smtpPassword)
        
        $message = New-Object System.Net.Mail.MailMessage
        $message.From = New-Object System.Net.Mail.MailAddress($senderEmail, $senderName)
        $message.To.Add($to)
        $message.Subject = $subject
        $message.IsBodyHtml = $true
        $message.Body = $body
        
        $smtp.Send($message)
        
        Write-Host "Correo electrónico enviado correctamente a $to" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "Error al enviar el correo electrónico: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Función para obtener la plantilla de correo electrónico
function Get-EmailTemplate {
    param (
        [string]$templateName
    )
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT HtmlContent FROM EmailTemplates WHERE Name = @Name"
    $command.Parameters.AddWithValue("@Name", $templateName) | Out-Null
    
    try {
        $connection.Open()
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
            return $reader["HtmlContent"]
        }
        else {
            Write-Host "No se encontró la plantilla de correo electrónico: $templateName" -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "Error al obtener la plantilla de correo electrónico: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
    finally {
        if ($reader) { $reader.Close() }
        $connection.Close()
    }
}

# Función para reemplazar variables en la plantilla
function Replace-TemplateVariables {
    param (
        [string]$template,
        [hashtable]$variables
    )
    
    foreach ($key in $variables.Keys) {
        $template = $template -replace "{{$key}}", $variables[$key]
    }
    
    return $template
}

# Función para generar un código de activación para un usuario
function Generate-UserActivationCode {
    param (
        [string]$email
    )
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $command = $connection.CreateCommand()
    $command.CommandText = "EXEC sp_GenerateActivationCode @Email"
    $command.Parameters.AddWithValue("@Email", $email) | Out-Null
    
    try {
        $connection.Open()
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
            $success = $reader["Success"]
            $activationCode = $reader["ActivationCode"]
            
            if ($success -eq 1) {
                Write-Host "Código de activación generado correctamente: $activationCode" -ForegroundColor Green
                return $activationCode
            }
            else {
                Write-Host "No se pudo generar el código de activación para el usuario: $email" -ForegroundColor Red
                return $null
            }
        }
        else {
            Write-Host "No se pudo generar el código de activación para el usuario: $email" -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "Error al generar el código de activación: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
    finally {
        if ($reader) { $reader.Close() }
        $connection.Close()
    }
}

# Función para obtener información del usuario
function Get-UserInfo {
    param (
        [string]$email
    )
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $command = $connection.CreateCommand()
    $command.CommandText = "SELECT Id, Username, FullName FROM Users WHERE Email = @Email"
    $command.Parameters.AddWithValue("@Email", $email) | Out-Null
    
    try {
        $connection.Open()
        $reader = $command.ExecuteReader()
        
        if ($reader.Read()) {
            $userInfo = @{
                Id = $reader["Id"]
                Username = $reader["Username"]
                FullName = $reader["FullName"]
                Email = $email
            }
            
            return $userInfo
        }
        else {
            Write-Host "No se encontró el usuario con el correo electrónico: $email" -ForegroundColor Red
            return $null
        }
    }
    catch {
        Write-Host "Error al obtener información del usuario: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
    finally {
        if ($reader) { $reader.Close() }
        $connection.Close()
    }
}

# Función principal para enviar un correo de activación
function Send-ActivationEmail {
    param (
        [string]$email
    )
    
    # Obtener información del usuario
    $userInfo = Get-UserInfo -email $email
    if ($null -eq $userInfo) { return $false }
    
    # Generar código de activación
    $activationCode = Generate-UserActivationCode -email $email
    if ($null -eq $activationCode) { 
        # Si no se pudo generar con el procedimiento almacenado, generamos uno manualmente
        $activationCode = Generate-ActivationCode
        Write-Host "Usando código de activación generado manualmente: $activationCode" -ForegroundColor Yellow
    }
    
    # Obtener la plantilla de correo electrónico
    $template = Get-EmailTemplate -templateName "ActivationCode"
    if ($null -eq $template) { return $false }
    
    # Reemplazar variables en la plantilla
    $variables = @{
        FullName = $userInfo.FullName
        Username = $userInfo.Username
        Email = $userInfo.Email
        ActivationCode = $activationCode
        ExpirationTime = "24 horas"
    }
    
    $body = Replace-TemplateVariables -template $template -variables $variables
    
    # Enviar el correo electrónico
    $subject = "Código de Activación - AuthSystem"
    return Send-Email -to $email -subject $subject -body $body
}

# Menú principal
function Show-Menu {
    Clear-Host
    Write-Host "=== Sistema de Activación de Usuarios ===" -ForegroundColor Cyan
    Write-Host "1. Enviar correo de activación a un usuario"
    Write-Host "2. Activar cuenta de usuario"
    Write-Host "3. Salir"
    Write-Host ""
}

# Bucle principal
$exit = $false
while (-not $exit) {
    Show-Menu
    $option = Read-Host "Seleccione una opción"
    
    switch ($option) {
        "1" {
            $email = Read-Host "Ingrese el correo electrónico del usuario"
            Send-ActivationEmail -email $email
            Write-Host ""
            Read-Host "Presione Enter para continuar"
        }
        "2" {
            $email = Read-Host "Ingrese el correo electrónico del usuario"
            $code = Read-Host "Ingrese el código de activación"
            
            $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
            $command = $connection.CreateCommand()
            $command.CommandText = "EXEC sp_ActivateUserAccount @Email, @ActivationCode"
            $command.Parameters.AddWithValue("@Email", $email) | Out-Null
            $command.Parameters.AddWithValue("@ActivationCode", $code) | Out-Null
            
            try {
                $connection.Open()
                $reader = $command.ExecuteReader()
                
                if ($reader.Read()) {
                    $isValid = $reader["IsValid"]
                    
                    if ($isValid -eq 1) {
                        Write-Host "Cuenta activada correctamente" -ForegroundColor Green
                    }
                    else {
                        Write-Host "Código de activación inválido o expirado" -ForegroundColor Red
                    }
                }
                else {
                    Write-Host "Error al activar la cuenta" -ForegroundColor Red
                }
            }
            catch {
                Write-Host "Error al activar la cuenta: $($_.Exception.Message)" -ForegroundColor Red
            }
            finally {
                if ($reader) { $reader.Close() }
                $connection.Close()
            }
            
            Write-Host ""
            Read-Host "Presione Enter para continuar"
        }
        "3" {
            $exit = $true
        }
        default {
            Write-Host "Opción inválida" -ForegroundColor Red
            Write-Host ""
            Read-Host "Presione Enter para continuar"
        }
    }
}
