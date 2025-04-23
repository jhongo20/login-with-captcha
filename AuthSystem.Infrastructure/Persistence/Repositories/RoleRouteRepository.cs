using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de RoleRoute
    /// </summary>
    public class RoleRouteRepository : IRoleRouteRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public RoleRouteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las rutas asignadas a un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de rutas</returns>
        public async Task<IEnumerable<Route>> GetRoutesByRoleAsync(Guid roleId)
        {
            return await _context.RoleRoutes
                .Where(rr => rr.RoleId == roleId && rr.IsActive)
                .Include(rr => rr.Route)
                .ThenInclude(r => r.Module)
                .Select(rr => rr.Route)
                .Where(r => r.IsActive)
                .OrderBy(r => r.Module.Name)
                .ThenBy(r => r.DisplayOrder)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los roles que tienen acceso a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Lista de roles</returns>
        public async Task<IEnumerable<Role>> GetRolesByRouteAsync(Guid routeId)
        {
            return await _context.RoleRoutes
                .Where(rr => rr.RouteId == routeId && rr.IsActive)
                .Include(rr => rr.Role)
                .ThenInclude(r => r.RolePermissions)
                .Select(rr => rr.Role)
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Asigna una ruta a un rol
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <returns>Task</returns>
        public async Task AssignRouteToRoleAsync(Guid routeId, Guid roleId, string userName)
        {
            try
            {
                // Verificar que la ruta y el rol existen
                var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == routeId && r.IsActive);
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId && r.IsActive);

                if (route == null)
                {
                    throw new InvalidOperationException($"La ruta con ID {routeId} no existe o no está activa");
                }

                if (role == null)
                {
                    throw new InvalidOperationException($"El rol con ID {roleId} no existe o no está activo");
                }

                // Verificar si ya existe la relación
                var existingRelation = await _context.RoleRoutes
                    .FirstOrDefaultAsync(rr => rr.RouteId == routeId && rr.RoleId == roleId);

                if (existingRelation != null)
                {
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
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en AssignRouteToRoleAsync: {ex.Message}");
                throw; // Re-lanzar la excepción para que el controlador pueda manejarla
            }
        }

        /// <summary>
        /// Revoca el acceso de un rol a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Task</returns>
        public async Task RevokeRouteFromRoleAsync(Guid routeId, Guid roleId)
        {
            try
            {
                var roleRoute = await _context.RoleRoutes
                    .FirstOrDefaultAsync(rr => rr.RouteId == routeId && rr.RoleId == roleId && rr.IsActive);

                if (roleRoute != null)
                {
                    roleRoute.IsActive = false;
                    roleRoute.LastModifiedAt = DateTime.UtcNow;
                    roleRoute.LastModifiedBy = "System"; // Idealmente, se debería pasar el usuario como parámetro

                    _context.RoleRoutes.Update(roleRoute);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en RevokeRouteFromRoleAsync: {ex.Message}");
                throw; // Re-lanzar la excepción para que el controlador pueda manejarla
            }
        }

        /// <summary>
        /// Verifica si un rol tiene acceso a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>True si el rol tiene acceso a la ruta, False en caso contrario</returns>
        public async Task<bool> RoleHasRouteAsync(Guid routeId, Guid roleId)
        {
            try
            {
                return await _context.RoleRoutes
                    .AnyAsync(rr => rr.RouteId == routeId && 
                                   rr.RoleId == roleId && 
                                   rr.IsActive);
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en RoleHasRouteAsync: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Obtiene todas las rutas de un módulo específico que son accesibles por un rol
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de rutas</returns>
        public async Task<IEnumerable<Route>> GetRoutesByModuleAndRoleAsync(Guid moduleId, Guid roleId)
        {
            try
            {
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
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en GetRoutesByModuleAndRoleAsync: {ex.Message}");
                // Devolver una lista vacía en caso de error
                return new List<Route>();
            }
        }
        
        /// <summary>
        /// Verifica si un rol tiene acceso a un módulo
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>True si el rol tiene acceso al módulo, False en caso contrario</returns>
        public async Task<bool> RoleHasModuleAccessAsync(Guid roleId, Guid moduleId)
        {
            // Verificar si el rol tiene el permiso "Modules.View" para cualquier módulo
            var hasModuleViewPermission = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && 
                               rp.Permission.Name == "Modules.View" && 
                               rp.IsActive &&
                               rp.Permission.IsActive);
                               
            if (!hasModuleViewPermission)
            {
                return false;
            }
            
            // Verificar si el rol tiene alguna asociación con el módulo específico
            var hasModuleAssociation = await _context.PermissionModules
                .AnyAsync(pm => pm.Module.Id == moduleId &&
                               _context.RolePermissions.Any(rp => 
                                   rp.RoleId == roleId && 
                                   rp.PermissionId == pm.PermissionId && 
                                   rp.IsActive));
                                   
            return hasModuleAssociation;
        }
    }
}