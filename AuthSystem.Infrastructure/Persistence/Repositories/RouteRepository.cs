using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    public class RouteRepository : GenericRepository<Route>, IRouteRepository
    {
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Route>> GetEnabledRoutesAsync()
        {
            try
            {
                // Modificamos la consulta para evitar el error de ModuleId1
                var routes = await _context.Routes
                    .Where(r => r.IsEnabled && r.IsActive)
                    .ToListAsync();
                
                // Cargamos los módulos manualmente para evitar problemas de relación
                foreach (var route in routes)
                {
                    route.Module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == route.ModuleId);
                }
                
                return routes.OrderBy(r => r.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEnabledRoutesAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                return new List<Route>();
            }
        }

        public async Task<IEnumerable<Route>> GetRoutesByModuleAsync(Guid moduleId)
        {
            try
            {
                // Modificamos la consulta para evitar el error de ModuleId1
                var routes = await _context.Routes
                    .Where(r => r.ModuleId == moduleId && r.IsActive)
                    .ToListAsync();
                
                // Cargamos los módulos manualmente para evitar problemas de relación
                foreach (var route in routes)
                {
                    route.Module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == route.ModuleId);
                }
                
                return routes.OrderBy(r => r.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetRoutesByModuleAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                return new List<Route>();
            }
        }

        public async Task<IEnumerable<Route>> GetRoutesByRoleAsync(Guid roleId)
        {
            try
            {
                // Modificamos la consulta para evitar el error de ModuleId1
                var roleRoutes = await _context.RoleRoutes
                    .Where(rr => rr.RoleId == roleId && rr.IsActive)
                    .ToListAsync();
                
                var routeIds = roleRoutes.Select(rr => rr.RouteId).ToList();
                
                var routes = await _context.Routes
                    .Where(r => routeIds.Contains(r.Id) && r.IsActive)
                    .ToListAsync();
                
                // Cargamos los módulos manualmente para evitar problemas de relación
                foreach (var route in routes)
                {
                    route.Module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == route.ModuleId);
                }
                
                return routes.OrderBy(r => r.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetRoutesByRoleAsync: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                return new List<Route>();
            }
        }

        public async Task<bool> RoleHasRouteAccessAsync(Guid roleId, Guid routeId)
        {
            return await _context.RoleRoutes
                .AnyAsync(rr => rr.RoleId == roleId && rr.RouteId == routeId && rr.IsActive);
        }

        public async Task<bool> RoleHasModuleAccessAsync(Guid roleId, Guid moduleId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Permission)
                .AnyAsync(rp => 
                    rp.RoleId == roleId && 
                    rp.IsActive && 
                    rp.Permission.Name == "Modules.View");
        }

        public async Task<bool> ExistsWithNameInModuleAsync(string name, Guid moduleId, Guid? excludeRouteId = null)
        {
            var query = _context.Routes
                .Where(r => r.Name == name && r.ModuleId == moduleId && r.IsActive);
                
            if (excludeRouteId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRouteId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task<bool> ExistsWithPathAndMethodAsync(string path, string httpMethod, Guid? excludeRouteId = null)
        {
            var query = _context.Routes
                .Where(r => r.Path == path && r.HttpMethod == httpMethod && r.IsActive);
                
            if (excludeRouteId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRouteId.Value);
            }
            
            return await query.AnyAsync();
        }

        public async Task AssignRouteToRoleAsync(Guid routeId, Guid roleId, string userName)
        {
            // Verificar si ya existe la relación
            var existingRelation = await _context.RoleRoutes
                .FirstOrDefaultAsync(rr => rr.RouteId == routeId && rr.RoleId == roleId);
                
            if (existingRelation != null)
            {
                // Si existe pero está inactiva, la reactivamos
                if (!existingRelation.IsActive)
                {
                    existingRelation.IsActive = true;
                    existingRelation.LastModifiedAt = DateTime.UtcNow;
                    existingRelation.LastModifiedBy = userName;
                    
                    _context.RoleRoutes.Update(existingRelation);
                    await _context.SaveChangesAsync();
                }
                // Si ya está activa, no hacemos nada
                return;
            }
            
            // Si no existe, creamos una nueva relación
            var roleRoute = new RoleRoute
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                RouteId = routeId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName,
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = userName
            };
            
            await _context.RoleRoutes.AddAsync(roleRoute);
            await _context.SaveChangesAsync();
        }

        public async Task RevokeRouteFromRoleAsync(Guid routeId, Guid roleId)
        {
            var roleRoute = await _context.RoleRoutes
                .FirstOrDefaultAsync(rr => rr.RouteId == routeId && rr.RoleId == roleId && rr.IsActive);
                
            if (roleRoute != null)
            {
                roleRoute.IsActive = false;
                roleRoute.LastModifiedAt = DateTime.UtcNow;
                
                _context.RoleRoutes.Update(roleRoute);
                await _context.SaveChangesAsync();
            }
        }

        public override async Task<IEnumerable<Route>> GetAllAsync()
        {
            try
            {
                // Modificamos la consulta para evitar el error de ModuleId1
                var routes = await _context.Routes
                    .Where(r => r.IsActive)
                    .ToListAsync();
                
                // Cargamos los módulos manualmente para evitar problemas de relación
                foreach (var route in routes)
                {
                    route.Module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == route.ModuleId);
                }
                
                return routes.OrderBy(r => r.DisplayOrder).ToList();
            }
            catch (Exception ex)
            {
                // Registrar el error con más detalles
                Console.WriteLine($"Error en GetAllAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                
                // Devolver una lista vacía en caso de error
                return new List<Route>();
            }
        }

        public override async Task<Route> GetByIdAsync(Guid id)
        {
            try
            {
                // Modificamos la consulta para evitar el error de ModuleId1
                var route = await _context.Routes
                    .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
                
                if (route != null)
                {
                    // Cargamos el módulo manualmente para evitar problemas de relación
                    route.Module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == route.ModuleId);
                }
                
                return route;
            }
            catch (DbUpdateException ex)
            {
                // Registrar el error de base de datos
                Console.WriteLine($"Error de base de datos en GetByIdAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                
                // Devolver null en caso de error de base de datos
                return null;
            }
            catch (Exception ex)
            {
                // Registrar el error con más detalles
                Console.WriteLine($"Error en GetByIdAsync: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }
                
                // Devolver null en caso de error
                return null;
            }
        }

        /// <summary>
        /// Obtiene todas las rutas de un módulo específico a las que tiene acceso un rol
        /// </summary>
        public async Task<IEnumerable<Route>> GetRoutesByModuleAndRoleAsync(Guid moduleId, Guid roleId)
        {
            // Verificar que el módulo y el rol existen
            var moduleExists = await _context.Modules.AnyAsync(m => m.Id == moduleId && m.IsActive);
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId && r.IsActive);

            if (!moduleExists || !roleExists)
            {
                return Enumerable.Empty<Route>();
            }

            // Obtener las rutas que pertenecen al módulo y están asignadas al rol
            return await _context.RoleRoutes
                .Where(rr => rr.RoleId == roleId && rr.IsActive)
                .Include(rr => rr.Route)
                .ThenInclude(r => r.Module)
                .Select(rr => rr.Route)
                .Where(r => r.ModuleId == moduleId && r.IsActive && r.IsEnabled)
                .OrderBy(r => r.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Asigna una ruta a un módulo
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <returns>Task</returns>
        public async Task AssignRouteToModuleAsync(Guid routeId, Guid moduleId, string userName)
        {
            // Verificar que la ruta y el módulo existen
            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == routeId && r.IsActive);
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId && m.IsActive);

            if (route == null)
            {
                throw new InvalidOperationException($"La ruta con ID {routeId} no existe o no está activa");
            }

            if (module == null)
            {
                throw new InvalidOperationException($"El módulo con ID {moduleId} no existe o no está activo");
            }

            // Actualizar el módulo de la ruta
            route.ModuleId = moduleId;
            route.LastModifiedAt = DateTime.UtcNow;
            route.LastModifiedBy = userName;

            _context.Routes.Update(route);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Revoca una ruta de un módulo (la desvincula)
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="userName">Nombre del usuario que realiza la revocación</param>
        /// <returns>Task</returns>
        public async Task RevokeRouteFromModuleAsync(Guid routeId, string userName)
        {
            // Verificar que la ruta existe
            var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == routeId && r.IsActive);

            if (route == null)
            {
                throw new InvalidOperationException($"La ruta con ID {routeId} no existe o no está activa");
            }

            // Verificar que la ruta está asignada a un módulo
            if (route.ModuleId == Guid.Empty)
            {
                throw new InvalidOperationException($"La ruta con ID {routeId} no está asignada a ningún módulo");
            }

            // Obtener un módulo "Sin Asignar" o crear uno si no existe
            var unassignedModule = await _context.Modules
                .FirstOrDefaultAsync(m => m.Name == "Sin Asignar" && m.IsActive);

            if (unassignedModule == null)
            {
                // Crear un módulo "Sin Asignar"
                unassignedModule = new Domain.Entities.Module
                {
                    Id = Guid.NewGuid(),
                    Name = "Sin Asignar",
                    Description = "Módulo para rutas sin asignación",
                    Route = "/unassigned",
                    Icon = "fa-question-circle",
                    DisplayOrder = 9999,
                    IsEnabled = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = userName
                };

                await _context.Modules.AddAsync(unassignedModule);
                await _context.SaveChangesAsync();
            }

            // Asignar la ruta al módulo "Sin Asignar"
            route.ModuleId = unassignedModule.Id;
            route.LastModifiedAt = DateTime.UtcNow;
            route.LastModifiedBy = userName;

            _context.Routes.Update(route);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtiene todas las rutas que no están vinculadas a ningún módulo
        /// </summary>
        /// <returns>Lista de rutas sin módulo</returns>
        public async Task<IEnumerable<Route>> GetRoutesWithoutModuleAsync()
        {
            // Obtener el módulo "Sin Asignar"
            var unassignedModule = await _context.Modules
                .FirstOrDefaultAsync(m => m.Name == "Sin Asignar" && m.IsActive);

            if (unassignedModule == null)
            {
                // Si no existe el módulo "Sin Asignar", devolver rutas con ModuleId nulo o Guid.Empty
                return await _context.Routes
                    .Where(r => r.ModuleId == Guid.Empty && r.IsActive)
                    .OrderBy(r => r.Name)
                    .ToListAsync();
            }

            // Devolver rutas asignadas al módulo "Sin Asignar"
            return await _context.Routes
                .Where(r => r.ModuleId == unassignedModule.Id && r.IsActive)
                .Include(r => r.Module)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        // Sobrescribir el método AddAsync para asegurarnos de que IsActive siempre sea true
        public override async Task AddAsync(Route entity)
        {
            // Forzar IsActive a true
            entity.IsActive = true;
            
            // Llamar al método base
            await base.AddAsync(entity);
        }
    }
}
