-- Template para notificación de cuenta bloqueada
UPDATE [dbo].[EmailTemplates]
SET [Subject] = 'Su cuenta ha sido bloqueada temporalmente',
    [HtmlContent] = '<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>Cuenta Bloqueada</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
        }
        .header {
            background-color: #e74c3c;
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
            background-color: #fdecea;
            border-left: 4px solid #e74c3c;
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
        <h1>Cuenta Bloqueada</h1>
    </div>
    <div class="content">
        <p>Estimado/a <strong>{{FullName}}</strong>,</p>
        
        <div class="alert">
            <p><strong>Su cuenta ha sido bloqueada temporalmente debido a múltiples intentos fallidos de inicio de sesión.</strong></p>
        </div>
        
        <p>Hemos detectado varios intentos fallidos de inicio de sesión en su cuenta, lo que ha activado nuestro sistema de seguridad. Esta medida es para proteger su cuenta contra accesos no autorizados.</p>
        
        <p><strong>Detalles del bloqueo:</strong></p>
        <ul>
            <li>Fecha y hora del bloqueo: {{LockoutDate}}</li>
            <li>Duración del bloqueo: {{LockoutDuration}}</li>
            <li>Su cuenta será desbloqueada automáticamente el: {{UnlockDate}}</li>
            <li>Dirección IP: {{IPAddress}}</li>
            <li>Ubicación aproximada: {{Location}}</li>
        </ul>
        
        <p>Si fue usted quien intentó iniciar sesión y olvidó su contraseña, puede solicitar un restablecimiento de contraseña una vez que su cuenta sea desbloqueada.</p>
        
        <p>Si no reconoce estos intentos de inicio de sesión, es posible que alguien esté intentando acceder a su cuenta. Le recomendamos que cambie su contraseña tan pronto como su cuenta sea desbloqueada.</p>
        
        <p style="text-align: center; margin-top: 20px;">
            <a href="{{ContactSupportUrl}}" class="button">Contactar a Soporte</a>
        </p>
        
        <p>Si necesita asistencia inmediata, por favor contacte a nuestro equipo de soporte en <a href="mailto:{{SupportEmail}}">{{SupportEmail}}</a>.</p>
        
        <p>Atentamente,<br/>
        El equipo de seguridad</p>
    </div>
    <div class="footer">
        <p>Este es un mensaje automático, por favor no responda a este correo.</p>
        <p>&copy; {{CurrentYear}} Sistema de Autenticación. Todos los derechos reservados.</p>
    </div>
</body>
</html>',
    [TextContent] = 'Estimado/a {{FullName}},

SU CUENTA HA SIDO BLOQUEADA TEMPORALMENTE DEBIDO A MÚLTIPLES INTENTOS FALLIDOS DE INICIO DE SESIÓN.

Hemos detectado varios intentos fallidos de inicio de sesión en su cuenta, lo que ha activado nuestro sistema de seguridad. Esta medida es para proteger su cuenta contra accesos no autorizados.

Detalles del bloqueo:
- Fecha y hora del bloqueo: {{LockoutDate}}
- Duración del bloqueo: {{LockoutDuration}}
- Su cuenta será desbloqueada automáticamente el: {{UnlockDate}}
- Dirección IP: {{IPAddress}}
- Ubicación aproximada: {{Location}}

Si fue usted quien intentó iniciar sesión y olvidó su contraseña, puede solicitar un restablecimiento de contraseña una vez que su cuenta sea desbloqueada.

Si no reconoce estos intentos de inicio de sesión, es posible que alguien esté intentando acceder a su cuenta. Le recomendamos que cambie su contraseña tan pronto como su cuenta sea desbloqueada.

Si necesita asistencia inmediata, por favor contacte a nuestro equipo de soporte en {{SupportEmail}}.

Atentamente,
El equipo de seguridad

Este es un mensaje automático, por favor no responda a este correo.
© {{CurrentYear}} Sistema de Autenticación. Todos los derechos reservados.',
    [Description] = 'Plantilla para notificar al usuario cuando su cuenta ha sido bloqueada temporalmente debido a múltiples intentos fallidos de inicio de sesión',
    [LastModifiedAt] = GETDATE(),
    [LastModifiedBy] = 'System'
WHERE [Name] = 'AccountLocked';
