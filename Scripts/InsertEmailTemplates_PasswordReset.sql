-- Template para solicitud de restablecimiento de contraseña
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
           ,'PasswordResetRequest'
           ,'Solicitud de restablecimiento de contraseña'
           ,'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Restablecimiento de Contraseña</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
        }
        .header {
            background-color: #4a6da7;
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
        .code {
            background-color: #f5f5f5;
            padding: 10px;
            border: 1px solid #ddd;
            font-family: monospace;
            font-size: 18px;
            text-align: center;
            margin: 15px 0;
            letter-spacing: 2px;
        }
        .button {
            display: inline-block;
            background-color: #4a6da7;
            color: white;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 15px;
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
        <h1>Restablecimiento de Contraseña</h1>
    </div>
    <div class="content">
        <p>Estimado/a <strong>{FullName}</strong>,</p>
        
        <p>Hemos recibido una solicitud para restablecer la contraseña de su cuenta. Si usted no realizó esta solicitud, por favor ignore este correo o contacte a soporte técnico.</p>
        
        <p>Para restablecer su contraseña, haga clic en el siguiente enlace:</p>
        
        <p style="text-align: center;">
            <a href="{ResetUrl}" class="button">Restablecer mi contraseña</a>
        </p>
        
        <p>O puede copiar y pegar la siguiente URL en su navegador:</p>
        
        <p class="code" style="word-break: break-all;">{ResetUrl}</p>
        
        <div class="warning">
            <p><strong>Importante:</strong> Este enlace expirará en {ExpirationTime} por razones de seguridad.</p>
        </div>
        
        <p>Si tiene problemas para restablecer su contraseña, por favor contacte a nuestro equipo de soporte.</p>
        
        <p>Atentamente,<br/>
        El equipo de soporte</p>
    </div>
    <div class="footer">
        <p>Este es un mensaje automático, por favor no responda a este correo.</p>
        <p>&copy; {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.</p>
    </div>
</body>
</html>'
           ,'Estimado/a {FullName},

Hemos recibido una solicitud para restablecer la contraseña de su cuenta. Si usted no realizó esta solicitud, por favor ignore este correo o contacte a soporte técnico.

Para restablecer su contraseña, visite el siguiente enlace:
{ResetUrl}

Importante: Este enlace expirará en {ExpirationTime} por razones de seguridad.

Si tiene problemas para restablecer su contraseña, por favor contacte a nuestro equipo de soporte.

Atentamente,
El equipo de soporte

Este es un mensaje automático, por favor no responda a este correo.
© {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.'
           ,'Plantilla para solicitud de restablecimiento de contraseña'
           ,1
           ,GETDATE()
           ,'System'
           ,GETDATE()
           ,'System');
