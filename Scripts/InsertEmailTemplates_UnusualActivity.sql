-- Template para notificación de actividad inusual
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
           ,'UnusualActivity'
           ,'Alerta de seguridad: Actividad inusual detectada'
           ,'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Actividad Inusual Detectada</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
        }
        .header {
            background-color: #e67e22;
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
            background-color: #fef5e7;
            border-left: 4px solid #e67e22;
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
        <h1>Alerta de Seguridad</h1>
    </div>
    <div class="content">
        <p>Estimado/a <strong>{FullName}</strong>,</p>
        
        <div class="alert">
            <p><strong>Hemos detectado actividad inusual en su cuenta.</strong></p>
        </div>
        
        <p>Nuestro sistema de seguridad ha detectado actividad potencialmente sospechosa en su cuenta el <strong>{ActivityDate}</strong>.</p>
        
        <p><strong>Detalles de la actividad:</strong></p>
        <ul>
            <li>Tipo de actividad: {ActivityType}</li>
            <li>Fecha y hora: {ActivityDate}</li>
            <li>Dirección IP: {IPAddress}</li>
            <li>Ubicación: {Location}</li>
            <li>Dispositivo: {Device}</li>
        </ul>
        
        <p>Si reconoce esta actividad, puede ignorar este mensaje.</p>
        
        <p>Si no reconoce esta actividad, le recomendamos que:</p>
        <ol>
            <li>Cambie su contraseña inmediatamente</li>
            <li>Revise la actividad reciente de su cuenta</li>
            <li>Habilite la autenticación de dos factores si aún no lo ha hecho</li>
            <li>Contacte a nuestro equipo de soporte</li>
        </ol>
        
        <p style="text-align: center; margin-top: 20px;">
            <a href="{SecuritySettingsUrl}" class="button">Revisar configuración de seguridad</a>
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

HEMOS DETECTADO ACTIVIDAD INUSUAL EN SU CUENTA.

Nuestro sistema de seguridad ha detectado actividad potencialmente sospechosa en su cuenta el {ActivityDate}.

Detalles de la actividad:
- Tipo de actividad: {ActivityType}
- Fecha y hora: {ActivityDate}
- Dirección IP: {IPAddress}
- Ubicación: {Location}
- Dispositivo: {Device}

Si reconoce esta actividad, puede ignorar este mensaje.

Si no reconoce esta actividad, le recomendamos que:
1. Cambie su contraseña inmediatamente
2. Revise la actividad reciente de su cuenta
3. Habilite la autenticación de dos factores si aún no lo ha hecho
4. Contacte a nuestro equipo de soporte

Atentamente,
El equipo de seguridad

Este es un mensaje automático, por favor no responda a este correo.
© {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.'
           ,'Plantilla para notificar al usuario cuando se detecta actividad inusual en su cuenta'
           ,1
           ,GETDATE()
           ,'System'
           ,GETDATE()
           ,'System');
