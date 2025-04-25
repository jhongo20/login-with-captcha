-- Script para insertar la plantilla de correo electrónico para notificaciones de inicio de sesión
-- Autor: Cascade AI
-- Fecha: 2024-04-25

-- Verificar si ya existe la plantilla
IF NOT EXISTS (SELECT 1 FROM [dbo].[EmailTemplates] WHERE [Name] = 'LoginNotification')
BEGIN
    -- Insertar la plantilla de correo electrónico para notificaciones de inicio de sesión
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlContent], [TextContent], [Description], [IsActive], 
        [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    )
    VALUES (
        NEWID(), -- Generar un nuevo GUID para el Id
        'LoginNotification', -- Nombre de la plantilla
        'Alerta de Seguridad: Nuevo inicio de sesión detectado', -- Asunto del correo
        
        -- Contenido HTML
        N'<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Alerta de Seguridad: Nuevo inicio de sesión detectado</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            margin: 0;
            padding: 0;
        }
        .container {
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }
        .header {
            background-color: #0056b3;
            color: white;
            padding: 20px;
            text-align: center;
        }
        .content {
            padding: 20px;
            background-color: #f9f9f9;
            border: 1px solid #ddd;
        }
        .footer {
            font-size: 12px;
            color: #777;
            text-align: center;
            margin-top: 20px;
            padding: 10px;
            background-color: #f1f1f1;
        }
        .alert-info {
            background-color: #f8f9fa;
            border-left: 4px solid #0056b3;
            padding: 15px;
            margin: 20px 0;
        }
        .btn {
            display: inline-block;
            padding: 10px 20px;
            background-color: #0056b3;
            color: white;
            text-decoration: none;
            border-radius: 4px;
            margin-top: 15px;
        }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>Alerta de Seguridad</h1>
        </div>
        <div class="content">
            <h2>Hola, {{FullName}}</h2>
            <p>Hemos detectado un nuevo inicio de sesión en tu cuenta el <strong>{{LoginDate}}</strong>.</p>
            
            <div class="alert-info">
                <p><strong>Detalles del inicio de sesión:</strong></p>
                <ul>
                    <li><strong>Dirección IP:</strong> {{IPAddress}}</li>
                    <li><strong>Ubicación aproximada:</strong> {{Location}}</li>
                    <li><strong>Dispositivo:</strong> {{Device}}</li>
                    <li><strong>Navegador:</strong> {{Browser}}</li>
                </ul>
            </div>
            
            <p>Si fuiste tú quien inició sesión, puedes ignorar este mensaje.</p>
            <p>Si <strong>NO reconoces esta actividad</strong>, te recomendamos:</p>
            <ol>
                <li>Cambiar tu contraseña inmediatamente</li>
                <li>Revisar la actividad reciente de tu cuenta</li>
                <li>Contactar al soporte técnico</li>
            </ol>
            
            <a href="{{SecuritySettingsUrl}}" class="btn">Revisar configuración de seguridad</a>
            
            <p style="margin-top: 30px;">Gracias,<br>Equipo de Seguridad</p>
        </div>
        <div class="footer">
            <p>Este es un mensaje automático, por favor no responda a este correo.</p>
            <p>&copy; {{CurrentYear}} Sistema de Autenticación. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>',

        -- Contenido de texto plano
        N'ALERTA DE SEGURIDAD: NUEVO INICIO DE SESIÓN DETECTADO

Hola, {{FullName}}

Hemos detectado un nuevo inicio de sesión en tu cuenta el {{LoginDate}}.

DETALLES DEL INICIO DE SESIÓN:
- Dirección IP: {{IPAddress}}
- Ubicación aproximada: {{Location}}
- Dispositivo: {{Device}}
- Navegador: {{Browser}}

Si fuiste tú quien inició sesión, puedes ignorar este mensaje.

Si NO reconoces esta actividad, te recomendamos:
1. Cambiar tu contraseña inmediatamente
2. Revisar la actividad reciente de tu cuenta
3. Contactar al soporte técnico

Para revisar tu configuración de seguridad, visita: {{SecuritySettingsUrl}}

Gracias,
Equipo de Seguridad

Este es un mensaje automático, por favor no responda a este correo.
© {{CurrentYear}} Sistema de Autenticación. Todos los derechos reservados.',
        
        'Plantilla para notificar a los usuarios sobre nuevos inicios de sesión en sus cuentas', -- Descripción
        1, -- IsActive = true
        GETDATE(), -- CreatedAt = fecha actual
        'System', -- CreatedBy
        GETDATE(), -- LastModifiedAt = fecha actual
        'System' -- LastModifiedBy
    );
    
    PRINT 'Plantilla de correo electrónico para notificaciones de inicio de sesión insertada correctamente.';
END
ELSE
BEGIN
    PRINT 'La plantilla de correo electrónico para notificaciones de inicio de sesión ya existe.';
END
