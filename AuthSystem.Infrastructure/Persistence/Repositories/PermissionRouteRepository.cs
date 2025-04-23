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
    /// Implementación del repositorio de PermissionRoute
    /// </summary>
    public class PermissionRouteRepository : IPermissionRouteRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public PermissionRouteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los permisos de una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Lista de permisos</returns>
        public async Task<IEnumerable<Permission>> GetPermissionsByRouteAsync(Guid routeId)
        {
            try
            {
                return await _context.PermissionRoutes
                    .Where(pr => pr.RouteId == routeId)
                    .Include(pr => pr.Permission)
                    .Select(pr => pr.Permission)
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en GetPermissionsByRouteAsync: {ex.Message}");
                // Devolver una lista vacía en caso de error
                return new List<Permission>();
            }
        }

        /// <summary>
        /// Obtiene todas las rutas que requieren un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Lista de rutas</returns>
        public async Task<IEnumerable<Route>> GetRoutesByPermissionAsync(Guid permissionId)
        {
            try
            {
                return await _context.PermissionRoutes
                    .Where(pr => pr.PermissionId == permissionId)
                    .Include(pr => pr.Route)
                    .ThenInclude(r => r.Module)
                    .Select(pr => pr.Route)
                    .Where(r => r.IsActive)
                    .OrderBy(r => r.Module.Name)
                    .ThenBy(r => r.DisplayOrder)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en GetRoutesByPermissionAsync: {ex.Message}");
                // Devolver una lista vacía en caso de error
                return new List<Route>();
            }
        }

        /// <summary>
        /// Asigna un permiso a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <returns>Task</returns>
        public async Task AssignPermissionToRouteAsync(Guid routeId, Guid permissionId, string userName)
        {
            try
            {
                // Verificar que la ruta y el permiso existen
                var route = await _context.Routes.FirstOrDefaultAsync(r => r.Id == routeId && r.IsActive);
                var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId && p.IsActive);

                if (route == null)
                {
                    throw new InvalidOperationException($"La ruta con ID {routeId} no existe o no está activa");
                }

                if (permission == null)
                {
                    throw new InvalidOperationException($"El permiso con ID {permissionId} no existe o no está activo");
                }

                // Verificar si ya existe la relación
                var existingRelation = await _context.PermissionRoutes
                    .FirstOrDefaultAsync(pr => pr.RouteId == routeId && pr.PermissionId == permissionId);

                if (existingRelation != null)
                {
                    // Si ya existe pero está inactiva, la reactivamos
                    if (!existingRelation.IsActive)
                    {
                        existingRelation.IsActive = true;
                        existingRelation.LastModifiedAt = DateTime.UtcNow;
                        existingRelation.LastModifiedBy = userName;
                        
                        _context.PermissionRoutes.Update(existingRelation);
                        await _context.SaveChangesAsync();
                    }
                    // Si ya está activa, no hacemos nada
                    return;
                }

                // Crear la relación
                var permissionRoute = new PermissionRoute
                {
                    Id = Guid.NewGuid(),
                    RouteId = routeId,
                    PermissionId = permissionId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = userName
                };

                await _context.PermissionRoutes.AddAsync(permissionRoute);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en AssignPermissionToRouteAsync: {ex.Message}");
                throw; // Re-lanzar la excepción para que el controlador pueda manejarla
            }
        }

        /// <summary>
        /// Revoca un permiso de una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Task</returns>
        public async Task RevokePermissionFromRouteAsync(Guid routeId, Guid permissionId)
        {
            try
            {
                // Buscar la relación
                var permissionRoute = await _context.PermissionRoutes
                    .FirstOrDefaultAsync(pr => pr.RouteId == routeId && pr.PermissionId == permissionId && pr.IsActive);

                if (permissionRoute == null)
                {
                    throw new InvalidOperationException($"El permiso no está asignado a la ruta o ya fue revocado");
                }

                // Desactivar la relación en lugar de eliminarla
                permissionRoute.IsActive = false;
                permissionRoute.LastModifiedAt = DateTime.UtcNow;
                permissionRoute.LastModifiedBy = "System"; // Idealmente, se debería pasar el usuario como parámetro

                _context.PermissionRoutes.Update(permissionRoute);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en RevokePermissionFromRouteAsync: {ex.Message}");
                throw; // Re-lanzar la excepción para que el controlador pueda manejarla
            }
        }

        /// <summary>
        /// Verifica si una ruta requiere un permiso específico
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>True si la ruta requiere el permiso, False en caso contrario</returns>
        public async Task<bool> RouteRequiresPermissionAsync(Guid routeId, Guid permissionId)
        {
            try
            {
                return await _context.PermissionRoutes
                    .AnyAsync(pr => pr.RouteId == routeId && 
                                   pr.PermissionId == permissionId && 
                                   pr.IsActive);
            }
            catch (Exception ex)
            {
                // Registrar el error
                Console.WriteLine($"Error en RouteRequiresPermissionAsync: {ex.Message}");
                return false;
            }
        }
    }
}
