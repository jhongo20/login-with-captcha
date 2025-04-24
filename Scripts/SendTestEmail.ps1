# Script para probar el envío de correos electrónicos directamente usando el servicio EmailService

# Configuración del correo electrónico
$smtpServer = "smtp.gmail.com"
$smtpPort = 587
$useSsl = $true
$username = "jhongopruebas@gmail.com"
$password = "tnoeowgsvuhfxfcb"
$senderName = "AuthSystem"
$senderEmail = "jhongopruebas@gmail.com"

# Destinatario y asunto
$recipientEmail = "jhongopruebas@gmail.com"
$subject = "Correo de prueba desde AuthSystem"

# Contenido del correo
$htmlContent = @"
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }
        .content { padding: 20px; background-color: #f9f9f9; }
        .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Correo de prueba</h1>
        </div>
        <div class="content">
            <p>Hola,</p>
            <p>Este es un correo de prueba enviado desde el sistema AuthSystem.</p>
            <p>Si estás recibiendo este correo, significa que la configuración de correo electrónico está funcionando correctamente.</p>
            <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
        </div>
        <div class="footer">
            <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
        </div>
    </div>
</body>
</html>
"@

$textContent = @"
Correo de prueba

Hola,

Este es un correo de prueba enviado desde el sistema AuthSystem.

Si estás recibiendo este correo, significa que la configuración de correo electrónico está funcionando correctamente.

Saludos cordiales,
El equipo de AuthSystem

Este es un correo electrónico automático, por favor no respondas a este mensaje.
"@

# Enviar correo usando MailKit directamente
Add-Type -Path "D:\Users\Jhon\Documentos\Mintrabajo\Modulo General usuarios\AuthSystemNew\packages\MailKit.4.3.0\lib\netstandard2.0\MailKit.dll"
Add-Type -Path "D:\Users\Jhon\Documentos\Mintrabajo\Modulo General usuarios\AuthSystemNew\packages\MimeKit.4.3.0\lib\netstandard2.0\MimeKit.dll"

try {
    # Crear el mensaje
    $message = New-Object MimeKit.MimeMessage
    $message.From.Add([MimeKit.MailboxAddress]::Parse("$senderName <$senderEmail>"))
    $message.To.Add([MimeKit.MailboxAddress]::Parse($recipientEmail))
    $message.Subject = $subject

    # Crear el cuerpo del mensaje
    $builder = New-Object MimeKit.BodyBuilder
    $builder.HtmlBody = $htmlContent
    $builder.TextBody = $textContent
    $message.Body = $builder.ToMessageBody()

    # Crear el cliente SMTP
    $client = New-Object MailKit.Net.Smtp.SmtpClient

    Write-Host "Conectando al servidor SMTP: $smtpServer:$smtpPort..."
    
    # Conectar al servidor SMTP
    $secureSocketOptions = [MailKit.Security.SecureSocketOptions]::StartTls
    $client.Connect($smtpServer, $smtpPort, $secureSocketOptions)
    
    Write-Host "Autenticando..."
    
    # Autenticar
    $client.Authenticate($username, $password)
    
    Write-Host "Enviando mensaje..."
    
    # Enviar el mensaje
    $client.Send($message)
    
    Write-Host "Desconectando..."
    
    # Desconectar
    $client.Disconnect($true)
    
    Write-Host "¡Correo enviado correctamente a $recipientEmail!" -ForegroundColor Green
}
catch {
    Write-Host "Error al enviar el correo electrónico: $_" -ForegroundColor Red
    Write-Host "Stack Trace: $($_.Exception.StackTrace)" -ForegroundColor Red
}
