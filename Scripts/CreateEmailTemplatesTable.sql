-- Script para crear la tabla EmailTemplates y sembrar datos iniciales

-- Crear la tabla EmailTemplates si no existe
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EmailTemplates')
BEGIN
    CREATE TABLE [dbo].[EmailTemplates](
        [Id] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Subject] [nvarchar](200) NOT NULL,
        [HtmlContent] [nvarchar](max) NOT NULL,
        [TextContent] [nvarchar](max) NOT NULL,
        [Description] [nvarchar](200) NULL,
        [IsActive] [bit] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](50) NOT NULL,
        [LastModifiedAt] [datetime2](7) NOT NULL,
        [LastModifiedBy] [nvarchar](50) NOT NULL,
        CONSTRAINT [PK_EmailTemplates] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    -- Crear índice único en el nombre de la plantilla
    CREATE UNIQUE NONCLUSTERED INDEX [IX_EmailTemplates_Name] ON [dbo].[EmailTemplates]
    (
        [Name] ASC
    );
END
GO

-- Sembrar datos iniciales (plantillas predefinidas)
-- Plantilla para la creación de usuarios
IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplates] WHERE [Name] = 'UserCreated')
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlContent], [TextContent], [Description], [IsActive], 
        [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    )
    VALUES (
        '8c7e0a8d-4b8d-4cd9-a6ed-7de69f4a5e8e', 
        'UserCreated',
        'Bienvenido a AuthSystem',
        N'<html>
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
                    <h1>¡Bienvenido a AuthSystem!</h1>
                </div>
                <div class="content">
                    <p>Hola <strong>{{FullName}}</strong>,</p>
                    <p>Tu cuenta ha sido creada exitosamente en nuestro sistema.</p>
                    <p>Detalles de tu cuenta:</p>
                    <ul>
                        <li><strong>Usuario:</strong> {{Username}}</li>
                        <li><strong>Correo electrónico:</strong> {{Email}}</li>
                        <li><strong>Fecha de creación:</strong> {{CurrentDate}}</li>
                    </ul>
                    <p>Ya puedes iniciar sesión en nuestra plataforma y comenzar a utilizarla.</p>
                    <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                </div>
                <div class="footer">
                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                </div>
            </div>
        </body>
        </html>',
        N'¡Bienvenido a AuthSystem!

        Hola {{FullName}},

        Tu cuenta ha sido creada exitosamente en nuestro sistema.

        Detalles de tu cuenta:
        - Usuario: {{Username}}
        - Correo electrónico: {{Email}}
        - Fecha de creación: {{CurrentDate}}

        Ya puedes iniciar sesión en nuestra plataforma y comenzar a utilizarla.

        Si tienes alguna pregunta, no dudes en contactarnos.

        Saludos cordiales,
        El equipo de AuthSystem

        Este es un correo electrónico automático, por favor no respondas a este mensaje.',
        'Plantilla para el correo de bienvenida cuando se crea un usuario',
        1, -- IsActive
        GETUTCDATE(), -- CreatedAt
        'System', -- CreatedBy
        GETUTCDATE(), -- LastModifiedAt
        'System' -- LastModifiedBy
    );
END

-- Plantilla para la actualización de usuarios
IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplates] WHERE [Name] = 'UserUpdated')
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlContent], [TextContent], [Description], [IsActive], 
        [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    )
    VALUES (
        '9d8f0b9e-5c9e-5de0-b7fe-8ef7a5f6b9f9', 
        'UserUpdated',
        'Tu cuenta ha sido actualizada',
        N'<html>
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
                    <h1>Actualización de Cuenta</h1>
                </div>
                <div class="content">
                    <p>Hola <strong>{{FullName}}</strong>,</p>
                    <p>Tu cuenta ha sido actualizada en nuestro sistema.</p>
                    <p>Detalles de tu cuenta:</p>
                    <ul>
                        <li><strong>Usuario:</strong> {{Username}}</li>
                        <li><strong>Correo electrónico:</strong> {{Email}}</li>
                        <li><strong>Fecha de actualización:</strong> {{UpdateDate}}</li>
                    </ul>
                    <p>Si no has realizado esta actualización o tienes alguna pregunta, por favor contacta a nuestro equipo de soporte inmediatamente.</p>
                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                </div>
                <div class="footer">
                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                </div>
            </div>
        </body>
        </html>',
        N'Actualización de Cuenta

        Hola {{FullName}},

        Tu cuenta ha sido actualizada en nuestro sistema.

        Detalles de tu cuenta:
        - Usuario: {{Username}}
        - Correo electrónico: {{Email}}
        - Fecha de actualización: {{UpdateDate}}

        Si no has realizado esta actualización o tienes alguna pregunta, por favor contacta a nuestro equipo de soporte inmediatamente.

        Saludos cordiales,
        El equipo de AuthSystem

        Este es un correo electrónico automático, por favor no respondas a este mensaje.',
        'Plantilla para el correo de notificación cuando se actualiza un usuario',
        1, -- IsActive
        GETUTCDATE(), -- CreatedAt
        'System', -- CreatedBy
        GETUTCDATE(), -- LastModifiedAt
        'System' -- LastModifiedBy
    );
END

-- Plantilla para el código de activación
IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplates] WHERE [Name] = 'ActivationCode')
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlContent], [TextContent], [Description], [IsActive], 
        [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    )
    VALUES (
        '7a6f0c7b-3a7c-4bc8-95ed-6de58f4a4e7d', 
        'ActivationCode',
        'Código de Activación - AuthSystem',
        N'<html>
        <head>
            <style>
                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }
                .content { padding: 20px; background-color: #f9f9f9; }
                .code { font-size: 24px; font-weight: bold; text-align: center; padding: 15px; background-color: #e9e9e9; margin: 20px 0; letter-spacing: 5px; }
                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">
                    <h1>Código de Activación</h1>
                </div>
                <div class="content">
                    <p>Hola <strong>{{FullName}}</strong>,</p>
                    <p>Has solicitado un código de activación para tu cuenta en AuthSystem.</p>
                    <p>Tu código de activación es:</p>
                    <div class="code">{{ActivationCode}}</div>
                    <p>Este código es válido por {{ExpirationTime}} a partir de ahora.</p>
                    <p>Si no has solicitado este código, por favor ignora este mensaje o contacta a nuestro equipo de soporte.</p>
                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                </div>
                <div class="footer">
                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                </div>
            </div>
        </body>
        </html>',
        N'Código de Activación

        Hola {{FullName}},

        Has solicitado un código de activación para tu cuenta en AuthSystem.

        Tu código de activación es: {{ActivationCode}}

        Este código es válido por {{ExpirationTime}} a partir de ahora.

        Si no has solicitado este código, por favor ignora este mensaje o contacta a nuestro equipo de soporte.

        Saludos cordiales,
        El equipo de AuthSystem

        Este es un correo electrónico automático, por favor no respondas a este mensaje.',
        'Plantilla para el correo con código de activación',
        1, -- IsActive
        GETUTCDATE(), -- CreatedAt
        'System', -- CreatedBy
        GETUTCDATE(), -- LastModifiedAt
        'System' -- LastModifiedBy
    );
END

-- Plantilla para el restablecimiento de contraseña
IF NOT EXISTS (SELECT * FROM [dbo].[EmailTemplates] WHERE [Name] = 'PasswordReset')
BEGIN
    INSERT INTO [dbo].[EmailTemplates] (
        [Id], [Name], [Subject], [HtmlContent], [TextContent], [Description], [IsActive], 
        [CreatedAt], [CreatedBy], [LastModifiedAt], [LastModifiedBy]
    )
    VALUES (
        '6b5e0d6a-2a6b-3ab7-84dc-5cf6b3e3a6c6', 
        'PasswordReset',
        'Restablecimiento de Contraseña - AuthSystem',
        N'<html>
        <head>
            <style>
                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
                .header { background-color: #4a6da7; color: white; padding: 10px 20px; text-align: center; }
                .content { padding: 20px; background-color: #f9f9f9; }
                .button { display: inline-block; padding: 10px 20px; background-color: #4a6da7; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }
                .footer { text-align: center; padding: 10px; font-size: 12px; color: #666; }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">
                    <h1>Restablecimiento de Contraseña</h1>
                </div>
                <div class="content">
                    <p>Hola <strong>{{FullName}}</strong>,</p>
                    <p>Has solicitado restablecer la contraseña de tu cuenta en AuthSystem.</p>
                    <p>Para restablecer tu contraseña, haz clic en el siguiente enlace:</p>
                    <p style="text-align: center;"><a href="{{ResetUrl}}" class="button">Restablecer Contraseña</a></p>
                    <p>O copia y pega la siguiente URL en tu navegador:</p>
                    <p>{{ResetUrl}}</p>
                    <p>Este enlace es válido por {{ExpirationTime}} a partir de ahora.</p>
                    <p>Si no has solicitado restablecer tu contraseña, por favor ignora este mensaje o contacta a nuestro equipo de soporte.</p>
                    <p>Saludos cordiales,<br>El equipo de AuthSystem</p>
                </div>
                <div class="footer">
                    <p>Este es un correo electrónico automático, por favor no respondas a este mensaje.</p>
                </div>
            </div>
        </body>
        </html>',
        N'Restablecimiento de Contraseña

        Hola {{FullName}},

        Has solicitado restablecer la contraseña de tu cuenta en AuthSystem.

        Para restablecer tu contraseña, visita el siguiente enlace:
        {{ResetUrl}}

        Este enlace es válido por {{ExpirationTime}} a partir de ahora.

        Si no has solicitado restablecer tu contraseña, por favor ignora este mensaje o contacta a nuestro equipo de soporte.

        Saludos cordiales,
        El equipo de AuthSystem

        Este es un correo electrónico automático, por favor no respondas a este mensaje.',
        'Plantilla para el correo de restablecimiento de contraseña',
        1, -- IsActive
        GETUTCDATE(), -- CreatedAt
        'System', -- CreatedBy
        GETUTCDATE(), -- LastModifiedAt
        'System' -- LastModifiedBy
    );
END
GO

-- Mensaje de confirmación
PRINT 'La tabla EmailTemplates ha sido creada y sembrada con éxito.'
GO
