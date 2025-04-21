-- Script para crear las tablas Routes y RoleRoutes

-- Crear tabla Routes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Routes')
BEGIN
    CREATE TABLE [dbo].[Routes](
        [Id] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [Path] [nvarchar](200) NOT NULL,
        [HttpMethod] [nvarchar](10) NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [RequiresAuth] [bit] NOT NULL,
        [IsEnabled] [bit] NOT NULL,
        [IsActive] [bit] NOT NULL,
        [ModuleId] [uniqueidentifier] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](100) NOT NULL,
        [LastModifiedAt] [datetime2](7) NOT NULL,
        [LastModifiedBy] [nvarchar](100) NULL,
        CONSTRAINT [PK_Routes] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Routes_Modules_ModuleId] FOREIGN KEY([ModuleId]) REFERENCES [dbo].[Modules] ([Id])
    );

    -- Crear índices para Routes
    CREATE NONCLUSTERED INDEX [IX_Routes_ModuleId] ON [dbo].[Routes]([ModuleId] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Routes_Name_ModuleId] ON [dbo].[Routes]([Name] ASC, [ModuleId] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Routes_Path_HttpMethod] ON [dbo].[Routes]([Path] ASC, [HttpMethod] ASC);
END

-- Crear tabla RoleRoutes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RoleRoutes')
BEGIN
    CREATE TABLE [dbo].[RoleRoutes](
        [Id] [uniqueidentifier] NOT NULL,
        [RoleId] [uniqueidentifier] NOT NULL,
        [RouteId] [uniqueidentifier] NOT NULL,
        [IsActive] [bit] NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] [nvarchar](100) NOT NULL,
        [LastModifiedAt] [datetime2](7) NOT NULL,
        [LastModifiedBy] [nvarchar](100) NULL,
        CONSTRAINT [PK_RoleRoutes] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_RoleRoutes_Roles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [dbo].[Roles] ([Id]),
        CONSTRAINT [FK_RoleRoutes_Routes_RouteId] FOREIGN KEY([RouteId]) REFERENCES [dbo].[Routes] ([Id])
    );

    -- Crear índices para RoleRoutes
    CREATE NONCLUSTERED INDEX [IX_RoleRoutes_RouteId] ON [dbo].[RoleRoutes]([RouteId] ASC);
    CREATE UNIQUE NONCLUSTERED INDEX [IX_RoleRoutes_RoleId_RouteId] ON [dbo].[RoleRoutes]([RoleId] ASC, [RouteId] ASC);
END
