-- Script para configurar la activación de usuarios
-- Este script actualiza la configuración para que los nuevos usuarios requieran activación

-- 1. Actualizar la tabla Users para que los usuarios existentes estén activos
UPDATE Users
SET EmailConfirmed = 1, IsActive = 1
WHERE EmailConfirmed = 0;

-- 2. Asegurarse de que la plantilla de correo electrónico para la activación exista
IF NOT EXISTS (SELECT 1 FROM EmailTemplates WHERE Name = 'ActivationCode')
BEGIN
    INSERT INTO EmailTemplates (Id, Name, Subject, HtmlContent, CreatedAt, CreatedBy)
    VALUES (
        NEWID(),
        'ActivationCode',
        'Código de Activación - AuthSystem',
        '<html>
        <head>
            <style>
                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
                .header { background-color: #4a6da7; color: white; padding: 10px 20px; }
                .content { padding: 20px; background-color: #f9f9f9; }
                .footer { text-align: center; font-size: 12px; color: #666; padding: 10px; }
                .code { font-size: 24px; font-weight: bold; color: #4a6da7; letter-spacing: 5px; }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">
                    <h2>Activación de Cuenta</h2>
                </div>
                <div class="content">
                    <p>Hola {{FullName}},</p>
                    <p>Gracias por registrarte en nuestro sistema. Para activar tu cuenta, por favor utiliza el siguiente código de activación:</p>
                    <p class="code">{{ActivationCode}}</p>
                    <p>Este código es válido por {{ExpirationTime}} a partir de ahora.</p>
                    <p>Si no has solicitado esta cuenta, por favor ignora este mensaje.</p>
                </div>
                <div class="footer">
                    <p>Este es un mensaje automático, por favor no responda a este correo.</p>
                    <p>&copy; 2025 AuthSystem. Todos los derechos reservados.</p>
                </div>
            </div>
        </body>
        </html>',
        GETDATE(),
        'System'
    );
END
ELSE
BEGIN
    -- Actualizar la plantilla existente para asegurarnos de que tenga el contenido correcto
    UPDATE EmailTemplates
    SET HtmlContent = '<html>
        <head>
            <style>
                body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
                .container { width: 100%; max-width: 600px; margin: 0 auto; padding: 20px; }
                .header { background-color: #4a6da7; color: white; padding: 10px 20px; }
                .content { padding: 20px; background-color: #f9f9f9; }
                .footer { text-align: center; font-size: 12px; color: #666; padding: 10px; }
                .code { font-size: 24px; font-weight: bold; color: #4a6da7; letter-spacing: 5px; }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">
                    <h2>Activación de Cuenta</h2>
                </div>
                <div class="content">
                    <p>Hola {{FullName}},</p>
                    <p>Gracias por registrarte en nuestro sistema. Para activar tu cuenta, por favor utiliza el siguiente código de activación:</p>
                    <p class="code">{{ActivationCode}}</p>
                    <p>Este código es válido por {{ExpirationTime}} a partir de ahora.</p>
                    <p>Si no has solicitado esta cuenta, por favor ignora este mensaje.</p>
                </div>
                <div class="footer">
                    <p>Este es un mensaje automático, por favor no responda a este correo.</p>
                    <p>&copy; 2025 AuthSystem. Todos los derechos reservados.</p>
                </div>
            </div>
        </body>
        </html>'
    WHERE Name = 'ActivationCode';
END

-- 3. Crear un procedimiento almacenado para la activación de cuentas
IF OBJECT_ID('sp_ActivateUserAccount', 'P') IS NOT NULL
    DROP PROCEDURE sp_ActivateUserAccount;
GO

CREATE PROCEDURE sp_ActivateUserAccount
    @Email NVARCHAR(256),
    @ActivationCode NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @IsValid BIT = 0;
    
    -- Verificar si el código de activación es válido
    SELECT @UserId = u.Id
    FROM Users u
    INNER JOIN ActivationCodes ac ON u.Id = ac.UserId
    WHERE u.Email = @Email
      AND ac.Code = @ActivationCode
      AND ac.IsUsed = 0
      AND ac.ExpiresAt > GETUTCDATE();
    
    IF @UserId IS NOT NULL
    BEGIN
        -- Activar la cuenta del usuario
        UPDATE Users
        SET EmailConfirmed = 1, IsActive = 1
        WHERE Id = @UserId;
        
        -- Marcar el código de activación como usado
        UPDATE ActivationCodes
        SET IsUsed = 1
        WHERE UserId = @UserId AND Code = @ActivationCode;
        
        SET @IsValid = 1;
    END
    
    -- Devolver el resultado
    SELECT @IsValid AS IsValid;
END
GO

-- 4. Crear un procedimiento almacenado para generar nuevos códigos de activación
IF OBJECT_ID('sp_GenerateActivationCode', 'P') IS NOT NULL
    DROP PROCEDURE sp_GenerateActivationCode;
GO

CREATE PROCEDURE sp_GenerateActivationCode
    @Email NVARCHAR(256),
    @CreatedBy NVARCHAR(256) = 'System'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId UNIQUEIDENTIFIER;
    DECLARE @ActivationCode NVARCHAR(10);
    DECLARE @Success BIT = 0;
    
    -- Obtener el ID del usuario
    SELECT @UserId = Id
    FROM Users
    WHERE Email = @Email;
    
    IF @UserId IS NOT NULL
    BEGIN
        -- Generar un código de activación aleatorio (6 caracteres)
        SET @ActivationCode = 
            CHAR(65 + CAST(RAND() * 26 AS INT)) +
            CHAR(65 + CAST(RAND() * 26 AS INT)) +
            CAST(CAST(RAND() * 10 AS INT) AS CHAR(1)) +
            CAST(CAST(RAND() * 10 AS INT) AS CHAR(1)) +
            CHAR(65 + CAST(RAND() * 26 AS INT)) +
            CAST(CAST(RAND() * 10 AS INT) AS CHAR(1));
        
        -- Desactivar códigos anteriores
        UPDATE ActivationCodes
        SET IsUsed = 1
        WHERE UserId = @UserId AND IsUsed = 0;
        
        -- Insertar el nuevo código de activación
        INSERT INTO ActivationCodes (Id, UserId, Code, ExpiresAt, IsUsed, CreatedAt, CreatedBy)
        VALUES (
            NEWID(),
            @UserId,
            @ActivationCode,
            DATEADD(HOUR, 24, GETUTCDATE()),
            0,
            GETUTCDATE(),
            @CreatedBy
        );
        
        SET @Success = 1;
    END
    
    -- Devolver el resultado
    SELECT @Success AS Success, @ActivationCode AS ActivationCode;
END
GO

PRINT 'Configuración de activación de usuarios completada.';
