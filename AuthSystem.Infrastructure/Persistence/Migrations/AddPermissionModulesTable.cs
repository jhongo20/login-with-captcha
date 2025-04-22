using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Migración para crear la tabla PermissionModules
    /// </summary>
    public class AddPermissionModulesTable
    {
        /// <summary>
        /// Ejecuta la migración
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public static void Execute(DbContext context)
        {
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PermissionModules')
                BEGIN
                    CREATE TABLE [dbo].[PermissionModules](
                        [Id] [uniqueidentifier] NOT NULL,
                        [PermissionId] [uniqueidentifier] NOT NULL,
                        [ModuleId] [uniqueidentifier] NOT NULL,
                        [CreatedAt] [datetime2](7) NOT NULL,
                        [CreatedBy] [nvarchar](50) NOT NULL,
                        [LastModifiedAt] [datetime2](7) NULL,
                        [LastModifiedBy] [nvarchar](50) NULL,
                        CONSTRAINT [PK_PermissionModules] PRIMARY KEY CLUSTERED 
                        (
                            [Id] ASC
                        ),
                        CONSTRAINT [IX_PermissionModules_PermissionId_ModuleId] UNIQUE NONCLUSTERED 
                        (
                            [PermissionId] ASC,
                            [ModuleId] ASC
                        ),
                        CONSTRAINT [FK_PermissionModules_Permissions_PermissionId] FOREIGN KEY([PermissionId])
                            REFERENCES [dbo].[Permissions] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_PermissionModules_Modules_ModuleId] FOREIGN KEY([ModuleId])
                            REFERENCES [dbo].[Modules] ([Id]) ON DELETE CASCADE
                    )
                END
            ");

            // Crear índices para mejorar el rendimiento
            context.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PermissionModules_ModuleId' AND object_id = OBJECT_ID('PermissionModules'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_PermissionModules_ModuleId] ON [dbo].[PermissionModules]
                    (
                        [ModuleId] ASC
                    )
                END
            ");
        }
    }
}
