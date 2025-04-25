-- Template para notificación de inicio de sesión
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
           ,'LoginNotification'
           ,'Inicio de sesión detectado en su cuenta'
           ,'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Inicio de sesión detectado</title>
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
        .alert {
            background-color: #fff8e1;
            border-left: 4px solid #ffc107;
            padding: 10px;
            margin: 15px 0;
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
    </style>
</head>
<body>
    <div class="header">
        <h1>Alerta de Seguridad</h1>
    </div>
    <div class="content">
        <p>Estimado/a <strong>{FullName}</strong>,</p>
        
        <p>Hemos detectado un inicio de sesión en su cuenta el <strong>{LoginDate}</strong> desde la dirección IP <strong>{IPAddress}</strong> utilizando <strong>{Browser}</strong>.</p>
        
        <div class="alert">
            <p><strong>Detalles del inicio de sesión:</strong></p>
            <ul>
                <li>Fecha y hora: {LoginDate}</li>
                <li>Dirección IP: {IPAddress}</li>
                <li>Ubicación aproximada: {Location}</li>
                <li>Dispositivo: {Device}</li>
                <li>Navegador: {Browser}</li>
            </ul>
        </div>
        
        <p>Si fue usted quien inició sesión, puede ignorar este mensaje.</p>
        
        <p>Si no reconoce esta actividad, le recomendamos que:</p>
        <ol>
            <li>Cambie su contraseña inmediatamente</li>
            <li>Revise la actividad reciente de su cuenta</li>
            <li>Contacte a soporte técnico</li>
        </ol>
        
        <a href="{SecuritySettingsUrl}" class="button">Revisar configuración de seguridad</a>
        
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

Hemos detectado un inicio de sesión en su cuenta el {LoginDate} desde la dirección IP {IPAddress} utilizando {Browser}.

Detalles del inicio de sesión:
- Fecha y hora: {LoginDate}
- Dirección IP: {IPAddress}
- Ubicación aproximada: {Location}
- Dispositivo: {Device}
- Navegador: {Browser}

Si fue usted quien inició sesión, puede ignorar este mensaje.

Si no reconoce esta actividad, le recomendamos que:
1. Cambie su contraseña inmediatamente
2. Revise la actividad reciente de su cuenta
3. Contacte a soporte técnico

Atentamente,
El equipo de seguridad

Este es un mensaje automático, por favor no responda a este correo.
© {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.'
           ,'Plantilla para notificar al usuario cuando se detecta un inicio de sesión en su cuenta'
           ,1
           ,GETDATE()
           ,'System'
           ,GETDATE()
           ,'System');
