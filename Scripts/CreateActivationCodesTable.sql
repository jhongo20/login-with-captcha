-- Script para crear la tabla de códigos de activación
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ActivationCodes')
BEGIN
    CREATE TABLE [dbo].[ActivationCodes](
        [Id] [uniqueidentifier] NOT NULL,
        [UserId] [uniqueidentifier] NOT NULL,
        [Code] [nvarchar](10) NOT NULL,
        [ExpiresAt] [datetime2](7) NOT NULL,
        [IsUsed] [bit] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](100) NULL,
        [UpdatedAt] [datetime2](7) NULL,
        [UpdatedBy] [nvarchar](100) NULL,
        CONSTRAINT [PK_ActivationCodes] PRIMARY KEY CLUSTERED 
        (
            [Id] ASC
        ),
        CONSTRAINT [FK_ActivationCodes_Users_UserId] FOREIGN KEY([UserId])
        REFERENCES [dbo].[Users] ([Id])
        ON DELETE CASCADE
    )

    PRINT 'Tabla ActivationCodes creada correctamente.'
END
ELSE
BEGIN
    PRINT 'La tabla ActivationCodes ya existe.'
END
