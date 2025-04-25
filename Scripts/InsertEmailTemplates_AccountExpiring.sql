-- Template para notificación de cuenta por expirar
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
           ,'AccountExpiring'
           ,'Su cuenta expirará pronto'
           ,'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Cuenta por Expirar</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
        }
        .header {
            background-color: #f39c12;
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
            background-color: #fcf8e3;
            border-left: 4px solid #f39c12;
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
        <h1>Cuenta por Expirar</h1>
    </div>
    <div class="content">
        <p>Estimado/a <strong>{FullName}</strong>,</p>
        
        <div class="alert">
            <p><strong>Su cuenta expirará en {DaysRemaining} días.</strong></p>
        </div>
        
        <p>Le informamos que su cuenta expirará el <strong>{ExpirationDate}</strong>. Para mantener el acceso a nuestros servicios, por favor realice una de las siguientes acciones:</p>
        
        <ol>
            <li>Inicie sesión en su cuenta antes de la fecha de expiración</li>
            <li>Actualice su información de perfil</li>
            <li>Contacte a nuestro equipo de soporte si necesita asistencia</li>
        </ol>
        
        <p>Si su cuenta expira, es posible que pierda acceso a ciertos datos y configuraciones.</p>
        
        <p style="text-align: center; margin-top: 20px;">
            <a href="{LoginUrl}" class="button">Iniciar Sesión Ahora</a>
        </p>
        
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

SU CUENTA EXPIRARÁ EN {DaysRemaining} DÍAS.

Le informamos que su cuenta expirará el {ExpirationDate}. Para mantener el acceso a nuestros servicios, por favor realice una de las siguientes acciones:

1. Inicie sesión en su cuenta antes de la fecha de expiración
2. Actualice su información de perfil
3. Contacte a nuestro equipo de soporte si necesita asistencia

Si su cuenta expira, es posible que pierda acceso a ciertos datos y configuraciones.

Atentamente,
El equipo de soporte

Este es un mensaje automático, por favor no responda a este correo.
© {CurrentYear} Sistema de Autenticación. Todos los derechos reservados.'
           ,'Plantilla para notificar al usuario cuando su cuenta está por expirar'
           ,1
           ,GETDATE()
           ,'System'
           ,GETDATE()
           ,'System');
