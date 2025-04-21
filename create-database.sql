-- Crear la base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'AuthSystemNewDb')
BEGIN
    CREATE DATABASE AuthSystemNewDb;
END
GO

USE AuthSystemNewDb;
GO

-- Configurar opciones SET
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- Crear tabla Users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()),
        Username NVARCHAR(50) NOT NULL,
        Email NVARCHAR(100) NOT NULL,
        FullName NVARCHAR(100) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        UserType INT NOT NULL,
        PhoneNumber NVARCHAR(20) NULL,
        PhoneNumberConfirmed BIT NOT NULL DEFAULT 0,
        EmailConfirmed BIT NOT NULL DEFAULT 0,
        TwoFactorEnabled BIT NOT NULL DEFAULT 0,
        LockoutEnd DATETIMEOFFSET NULL,
        LockoutEnabled BIT NOT NULL DEFAULT 1,
        AccessFailedCount INT NOT NULL DEFAULT 0,
        RefreshToken NVARCHAR(MAX) NULL,
        RefreshTokenExpiryTime DATETIMEOFFSET NULL,
        LastLoginDate DATETIMEOFFSET NULL,
        CreatedBy NVARCHAR(100) NULL,
        CreatedDate DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
        LastModifiedBy NVARCHAR(100) NULL,
        LastModifiedDate DATETIMEOFFSET NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    
    CREATE UNIQUE INDEX IX_Users_Username ON Users(Username) WHERE IsDeleted = 0;
    CREATE UNIQUE INDEX IX_Users_Email ON Users(Email) WHERE IsDeleted = 0;
END
GO

-- Crear tabla Roles
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()),
        Name NVARCHAR(50) NOT NULL,
        Description NVARCHAR(200) NULL,
        CreatedBy NVARCHAR(100) NULL,
        CreatedDate DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
        LastModifiedBy NVARCHAR(100) NULL,
        LastModifiedDate DATETIMEOFFSET NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    
    CREATE UNIQUE INDEX IX_Roles_Name ON Roles(Name) WHERE IsDeleted = 0;
END
GO

-- Crear tabla Permissions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
    CREATE TABLE Permissions (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()),
        Name NVARCHAR(100) NOT NULL,
        Description NVARCHAR(200) NULL,
        Module NVARCHAR(50) NOT NULL,
        CreatedBy NVARCHAR(100) NULL,
        CreatedDate DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
        LastModifiedBy NVARCHAR(100) NULL,
        LastModifiedDate DATETIMEOFFSET NULL,
        IsDeleted BIT NOT NULL DEFAULT 0
    );
    
    CREATE UNIQUE INDEX IX_Permissions_Name ON Permissions(Name) WHERE IsDeleted = 0;
END
GO

-- Crear tabla UserRoles (relación muchos a muchos entre Users y Roles)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserRoles')
BEGIN
    CREATE TABLE UserRoles (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()),
        UserId UNIQUEIDENTIFIER NOT NULL,
        RoleId UNIQUEIDENTIFIER NOT NULL,
        CreatedBy NVARCHAR(100) NULL,
        CreatedDate DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
        LastModifiedBy NVARCHAR(100) NULL,
        LastModifiedDate DATETIMEOFFSET NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES Users(Id),
        CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
    );
    
    CREATE UNIQUE INDEX IX_UserRoles_UserId_RoleId ON UserRoles(UserId, RoleId) WHERE IsDeleted = 0;
END
GO

-- Crear tabla RolePermissions (relación muchos a muchos entre Roles y Permissions)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RolePermissions')
BEGIN
    CREATE TABLE RolePermissions (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()),
        RoleId UNIQUEIDENTIFIER NOT NULL,
        PermissionId UNIQUEIDENTIFIER NOT NULL,
        CreatedBy NVARCHAR(100) NULL,
        CreatedDate DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
        LastModifiedBy NVARCHAR(100) NULL,
        LastModifiedDate DATETIMEOFFSET NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id),
        CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
    );
    
    CREATE UNIQUE INDEX IX_RolePermissions_RoleId_PermissionId ON RolePermissions(RoleId, PermissionId) WHERE IsDeleted = 0;
END
GO

-- Crear tabla UserSessions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserSessions')
BEGIN
    CREATE TABLE UserSessions (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT (NEWID()),
        UserId UNIQUEIDENTIFIER NOT NULL,
        Token NVARCHAR(MAX) NOT NULL,
        RefreshToken NVARCHAR(MAX) NULL,
        IpAddress NVARCHAR(50) NULL,
        UserAgent NVARCHAR(500) NULL,
        ExpiryDate DATETIMEOFFSET NOT NULL,
        CreatedDate DATETIMEOFFSET NOT NULL DEFAULT (SYSDATETIMEOFFSET()),
        IsRevoked BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_UserSessions_Users FOREIGN KEY (UserId) REFERENCES Users(Id)
    );
END
GO

-- Insertar datos semilla
-- Roles predeterminados
IF NOT EXISTS (SELECT * FROM Roles WHERE Name = 'Admin' AND IsDeleted = 0)
BEGIN
    INSERT INTO Roles (Id, Name, Description, CreatedBy, CreatedDate)
    VALUES (NEWID(), 'Admin', 'Administrador del sistema con acceso completo', 'System', SYSDATETIMEOFFSET());
END
GO

IF NOT EXISTS (SELECT * FROM Roles WHERE Name = 'User' AND IsDeleted = 0)
BEGIN
    INSERT INTO Roles (Id, Name, Description, CreatedBy, CreatedDate)
    VALUES (NEWID(), 'User', 'Usuario estándar con acceso limitado', 'System', SYSDATETIMEOFFSET());
END
GO

-- Permisos básicos
IF NOT EXISTS (SELECT * FROM Permissions WHERE Name = 'Users.View' AND IsDeleted = 0)
BEGIN
    INSERT INTO Permissions (Id, Name, Description, Module, CreatedBy, CreatedDate)
    VALUES (NEWID(), 'Users.View', 'Ver usuarios', 'Users', 'System', SYSDATETIMEOFFSET());
END
GO

IF NOT EXISTS (SELECT * FROM Permissions WHERE Name = 'Users.Create' AND IsDeleted = 0)
BEGIN
    INSERT INTO Permissions (Id, Name, Description, Module, CreatedBy, CreatedDate)
    VALUES (NEWID(), 'Users.Create', 'Crear usuarios', 'Users', 'System', SYSDATETIMEOFFSET());
END
GO

IF NOT EXISTS (SELECT * FROM Permissions WHERE Name = 'Users.Edit' AND IsDeleted = 0)
BEGIN
    INSERT INTO Permissions (Id, Name, Description, Module, CreatedBy, CreatedDate)
    VALUES (NEWID(), 'Users.Edit', 'Editar usuarios', 'Users', 'System', SYSDATETIMEOFFSET());
END
GO

IF NOT EXISTS (SELECT * FROM Permissions WHERE Name = 'Users.Delete' AND IsDeleted = 0)
BEGIN
    INSERT INTO Permissions (Id, Name, Description, Module, CreatedBy, CreatedDate)
    VALUES (NEWID(), 'Users.Delete', 'Eliminar usuarios', 'Users', 'System', SYSDATETIMEOFFSET());
END
GO

-- Usuario administrador predeterminado (contraseña: Admin123!)
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin' AND IsDeleted = 0)
BEGIN
    DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
    DECLARE @AdminRoleId UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Admin' AND IsDeleted = 0);
    
    INSERT INTO Users (Id, Username, Email, FullName, PasswordHash, UserType, EmailConfirmed, CreatedBy, CreatedDate)
    VALUES (@AdminId, 'admin', 'admin@example.com', 'Administrador', '$2a$11$iqJSHD.BGr0E2IxQwYgJmeP3NvhPrXAeLSaGCj6IR/XU5QtjVu5Tm', 0, 1, 'System', SYSDATETIMEOFFSET());
    
    INSERT INTO UserRoles (Id, UserId, RoleId, CreatedBy, CreatedDate)
    VALUES (NEWID(), @AdminId, @AdminRoleId, 'System', SYSDATETIMEOFFSET());
END
GO

-- Asignar permisos al rol de administrador
DECLARE @AdminRoleId2 UNIQUEIDENTIFIER = (SELECT Id FROM Roles WHERE Name = 'Admin' AND IsDeleted = 0);
DECLARE @PermissionIds TABLE (Id UNIQUEIDENTIFIER);
INSERT INTO @PermissionIds SELECT Id FROM Permissions WHERE IsDeleted = 0;
DECLARE @PermissionId UNIQUEIDENTIFIER;

DECLARE permission_cursor CURSOR FOR 
SELECT Id FROM @PermissionIds;

OPEN permission_cursor;
FETCH NEXT FROM permission_cursor INTO @PermissionId;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF NOT EXISTS (SELECT * FROM RolePermissions WHERE RoleId = @AdminRoleId2 AND PermissionId = @PermissionId AND IsDeleted = 0)
    BEGIN
        INSERT INTO RolePermissions (Id, RoleId, PermissionId, CreatedBy, CreatedDate)
        VALUES (NEWID(), @AdminRoleId2, @PermissionId, 'System', SYSDATETIMEOFFSET());
    END
    
    FETCH NEXT FROM permission_cursor INTO @PermissionId;
END

CLOSE permission_cursor;
DEALLOCATE permission_cursor;
GO

PRINT 'Base de datos AuthSystemNewDb creada correctamente con todas las tablas y datos semilla.';
