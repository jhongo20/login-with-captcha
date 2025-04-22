using System;
using System.Threading.Tasks;
using AuthSystem.Infrastructure.Persistence.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AuthSystem.Infrastructure.Persistence
{
    /// <summary>
    /// Inicializador de la base de datos
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>
        /// Inicializa la base de datos
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios</param>
        /// <returns>Task</returns>
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<DatabaseInitializer>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            try
            {
                logger.LogInformation("Inicializando la base de datos...");

                // Aplicar migraciones
                await context.Database.MigrateAsync();

                // Ejecutar migraciones personalizadas
                await ExecuteCustomMigrationsAsync(context, logger);

                logger.LogInformation("Base de datos inicializada correctamente");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al inicializar la base de datos");
                throw;
            }
        }

        /// <summary>
        /// Ejecuta migraciones personalizadas
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        /// <param name="logger">Logger</param>
        /// <returns>Task</returns>
        private static async Task ExecuteCustomMigrationsAsync(ApplicationDbContext context, ILogger<DatabaseInitializer> logger)
        {
            try
            {
                // Crear tabla PermissionModules
                logger.LogInformation("Creando tabla PermissionModules...");
                AddPermissionModulesTable.Execute(context);
                
                // Agregar permiso Modules.View
                logger.LogInformation("Agregando permiso Modules.View...");
                AddModuleViewPermission.Execute(context);

                await context.SaveChangesAsync();
                logger.LogInformation("Migraciones personalizadas ejecutadas correctamente");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al ejecutar migraciones personalizadas");
                throw;
            }
        }
    }
}
