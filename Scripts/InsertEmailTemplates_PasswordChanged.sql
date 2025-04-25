-- Template para notificación de cambio de contraseña
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
           ,'PasswordChanged'
           ,'Su contraseña ha sido cambiada'
           ,'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Cambio de Contraseña</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
        }
        .header {
            background-color: #2ecc71;
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
        .alert {
            background-color: #eafaf1;
            border-left: 4px solid #2ecc71;
            padding: 10px;
            margin: 15px 0;
        }
        .button {
            display: inline-block;
            background-color: #3498db;
            color: white;
            padding: 10px 20px;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 15px;
        }
    </style>
</head>
<body>
    <div class="header">
        <h1>Cambio de Contraseña</h1>
    </div>
    <div class="content">
        <p>Estimado/a <strong>{FullName}</strong>,</p>
        
        <div class="alert">
            <p><strong>La contraseña de su cuenta ha sido cambiada exitosamente.</strong></p>
        </div>
        
        <p>Le informamos que la contraseña de su cuenta ha sido cambiada el <strong>{ChangeDate}</strong>.</p>
        
        <p><strong>Detalles del cambio:</strong></p>
        <ul>
            <li>Fecha y hora: {ChangeDate}</li>
            <li>Dirección IP: {IPAddress}</li>
            <li>Dispositivo: {Device}</li>
        </ul>
        
        <p>Si usted realizó este cambio, puede ignorar este mensaje.</p>
        
        <p>Si no reconoce esta actividad, por favor contacte inmediatamente a nuestro equipo de soporte, ya que esto podría indicar que alguien ha accedido a su cuenta sin autorización.</p>
        
        <p style="text-align: center; margin-top: 20px;">
            <a href="{ContactSupportUrl}" class="button">Contactar a Soporte</a>
        </p>
        
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

LA CONTRASEÑA DE SU CUENTA HA SIDO CAMBIADA EXITOSAMENTE.

Le informamos que la contraseña de su cuenta ha sido cambiada el {ChangeDate}.

Detalles del cambio:
- Fecha y hora: {ChangeDate}
- Dirección IP: {IPAddress}
- Dispositivo: {Device}

Si usted realizó este cambio, puede ignorar este mensaje.

Si no reconoce esta actividad, por favor contacte inmediatamente a nuestro equipo de soporte, ya que esto podría indicar que alguien ha accedido a su cuenta sin autorización.

Atentamente,
El equipo de seguridad

Este es un mensaje automático, por favor no responda a este correo.
© {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.'
           ,'Plantilla para notificar al usuario cuando su contraseña ha sido cambiada'
           ,1
           ,GETDATE()
           ,'System'
           ,GETDATE()
           ,'System');
