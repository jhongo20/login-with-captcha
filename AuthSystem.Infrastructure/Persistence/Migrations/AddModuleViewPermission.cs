using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Migración para agregar el permiso Modules.View
    /// </summary>
    public class AddModuleViewPermission
    {
        /// <summary>
        /// Ejecuta la migración
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public static void Execute(DbContext context)
        {
            // Verificar si el permiso ya existe
            var permissionExists = context.Database.ExecuteSqlRaw(@"
                IF EXISTS (SELECT 1 FROM [Permissions] WHERE [Name] = 'Modules.View')
                    SELECT 1
                ELSE
                    SELECT 0
            ") > 0;

            if (!permissionExists)
            {
                // Agregar el permiso Modules.View
                var permissionId = Guid.NewGuid();
                context.Database.ExecuteSqlRaw($@"
                    INSERT INTO [dbo].[Permissions] (
                        [Id], 
                        [Name], 
                        [Description], 
                        [IsActive], 
                        [CreatedAt], 
                        [CreatedBy], 
                        [LastModifiedAt], 
                        [LastModifiedBy]
                    )
                    VALUES (
                        '{permissionId}', 
                        'Modules.View', 
                        'Permite ver módulos en el sistema', 
                        1, 
                        GETUTCDATE(), 
                        'System', 
                        NULL, 
                        NULL
                    )
                ");

                // Asignar el permiso al rol Admin
                context.Database.ExecuteSqlRaw($@"
                    INSERT INTO [dbo].[RolePermissions] (
                        [RoleId], 
                        [PermissionId], 
                        [IsActive], 
                        [CreatedAt], 
                        [CreatedBy], 
                        [LastModifiedAt], 
                        [LastModifiedBy]
                    )
                    SELECT 
                        [Id], 
                        '{permissionId}', 
                        1, 
                        GETUTCDATE(), 
                        'System', 
                        NULL, 
                        NULL
                    FROM [dbo].[Roles]
                    WHERE [Name] = 'Admin'
                ");
            }
        }
    }
}
