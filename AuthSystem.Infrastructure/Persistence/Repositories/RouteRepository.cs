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
            return await _context.Routes
                .Where(r => r.IsEnabled && r.IsActive)
                .Include(r => r.Module)
                .OrderBy(r => r.Module.Name)
                .ThenBy(r => r.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<Route>> GetRoutesByModuleAsync(Guid moduleId)
        {
            return await _context.Routes
                .Where(r => r.ModuleId == moduleId && r.IsActive)
                .Include(r => r.Module)
                .OrderBy(r => r.DisplayOrder)
                .ToListAsync();
        }

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
            return await _context.Routes
                .Where(r => r.IsActive)
                .Include(r => r.Module)
                .OrderBy(r => r.Module.Name)
                .ThenBy(r => r.DisplayOrder)
                .ToListAsync();
        }

        public override async Task<Route> GetByIdAsync(Guid id)
        {
            return await _context.Routes
                .Include(r => r.Module)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);
        }
    }
}
