-- Template para código de autenticación de dos factores
INSERT INTO [dbo].[EmailTemplates]
           ([Id]
           ,[Name]
           ,[Subject]
           ,[HtmlContent]
           ,[TextContent]
           ,[Description]
           ,[IsActive]
           ,[CreatedAt]
           ,[CreatedBy]
           ,[LastModifiedAt]
           ,[LastModifiedBy])
     VALUES
           (NEWID()
           ,'TwoFactorAuthCode'
           ,'Código de verificación para autenticación de dos factores'
           ,'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Código de Verificación</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
        }
        .header {
            background-color: #3498db;
            color: white;
            padding: 20px;
            text-align: center;
        }
        .content {
            padding: 20px;
            border: 1px solid #ddd;
        }
        .footer {
            background-color: #f5f5f5;
            padding: 10px;
            text-align: center;
            font-size: 12px;
            color: #666;
        }
        .code-container {
            background-color: #f8f9fa;
            border: 1px solid #ddd;
            border-radius: 4px;
            padding: 15px;
            margin: 20px 0;
            text-align: center;
        }
        .verification-code {
            font-family: monospace;
            font-size: 24px;
            letter-spacing: 5px;
            font-weight: bold;
            color: #333;
        }
        .warning {
            color: #856404;
            background-color: #fff3cd;
            border-left: 4px solid #ffc107;
            padding: 10px;
            margin: 15px 0;
        }
    </style>
</head>
<body>
    <div class="header">
        <h1>Código de Verificación</h1>
    </div>
    <div class="content">
        <p>Estimado/a <strong>{FullName}</strong>,</p>
        
        <p>Hemos recibido una solicitud de inicio de sesión en su cuenta que requiere verificación adicional. Para completar el proceso de autenticación de dos factores, utilice el siguiente código:</p>
        
        <div class="code-container">
            <p class="verification-code">{VerificationCode}</p>
        </div>
        
        <div class="warning">
            <p><strong>Importante:</strong> Este código expirará en {ExpirationTime} minutos por razones de seguridad. No comparta este código con nadie.</p>
        </div>
        
        <p><strong>Detalles del intento de inicio de sesión:</strong></p>
        <ul>
            <li>Fecha y hora: {LoginAttemptDate}</li>
            <li>Dirección IP: {IPAddress}</li>
            <li>Ubicación aproximada: {Location}</li>
            <li>Dispositivo: {Device}</li>
        </ul>
        
        <p>Si no intentó iniciar sesión, por favor ignore este correo y considere cambiar su contraseña inmediatamente.</p>
        
        <p>Atentamente,<br/>
        El equipo de seguridad</p>
    </div>
    <div class="footer">
        <p>Este es un mensaje automático, por favor no responda a este correo.</p>
        <p>&copy; {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.</p>
    </div>
</body>
</html>'
           ,'Estimado/a {FullName},

Hemos recibido una solicitud de inicio de sesión en su cuenta que requiere verificación adicional. Para completar el proceso de autenticación de dos factores, utilice el siguiente código:

{VerificationCode}

IMPORTANTE: Este código expirará en {ExpirationTime} minutos por razones de seguridad. No comparta este código con nadie.

Detalles del intento de inicio de sesión:
- Fecha y hora: {LoginAttemptDate}
- Dirección IP: {IPAddress}
- Ubicación aproximada: {Location}
- Dispositivo: {Device}

Si no intentó iniciar sesión, por favor ignore este correo y considere cambiar su contraseña inmediatamente.

Atentamente,
El equipo de seguridad

Este es un mensaje automático, por favor no responda a este correo.
© {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.'
           ,'Plantilla para enviar código de verificación para autenticación de dos factores'
           ,1
           ,GETDATE()
           ,'System'
           ,GETDATE()
           ,'System');
