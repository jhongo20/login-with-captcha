using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            // Verificar si el permiso ya existe usando una consulta directa
            var sql = "SELECT COUNT(1) FROM [Permissions] WHERE [Name] = 'Modules.View'";
            var count = 0;
            
            try 
            {
                count = Convert.ToInt32(context.Database.ExecuteSqlRaw(sql));
            }
            catch
            {
                // Si hay un error en la consulta, asumimos que el permiso no existe
                count = 0;
            }

            if (count == 0)
            {
                // Agregar el permiso Modules.View
                var permissionId = Guid.NewGuid();
                try
                {
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
                catch (Exception ex)
                {
                    // Registrar el error pero no detener la ejecución
                    Console.WriteLine($"Error al agregar el permiso Modules.View: {ex.Message}");
                }
            }
        }
    }
}
