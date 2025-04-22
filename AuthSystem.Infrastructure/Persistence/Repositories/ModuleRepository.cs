using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de módulos
    /// </summary>
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public ModuleRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene todos los módulos principales (sin padre)
        /// </summary>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos principales</returns>
        public async Task<IEnumerable<Module>> GetRootModulesAsync(bool includeChildren = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Module> query = _dbSet.Where(m => m.ParentId == null);

            if (includeChildren)
            {
                query = query.Include(m => m.Children);
            }

            return await query.OrderBy(m => m.DisplayOrder).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los submódulos de un módulo padre
        /// </summary>
        /// <param name="parentId">ID del módulo padre</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de submódulos</returns>
        public async Task<IEnumerable<Module>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los módulos habilitados
        /// </summary>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos habilitados</returns>
        public async Task<IEnumerable<Module>> GetEnabledModulesAsync(bool includeChildren = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Module> query = _dbSet.Where(m => m.IsEnabled);

            if (includeChildren)
            {
                query = query.Include(m => m.Children.Where(c => c.IsEnabled));
            }

            return await query.OrderBy(m => m.DisplayOrder).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Verifica si un módulo tiene submódulos
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si tiene submódulos, False en caso contrario</returns>
        public async Task<bool> HasChildrenAsync(Guid moduleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(m => m.ParentId == moduleId, cancellationToken);
        }

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad encontrada o null</returns>
        public async Task<Module> GetByIdWithChildrenAsync(Guid id, bool includeChildren = false, CancellationToken cancellationToken = default)
        {
            IQueryable<Module> query = _dbSet;

            if (includeChildren)
            {
                query = query.Include(m => m.Children);
            }

            return await query.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los módulos asignados a un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos asignados al rol</returns>
        public async Task<IEnumerable<Module>> GetModulesByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            // Verificar que el rol existe
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId && r.IsActive, cancellationToken);
            if (!roleExists)
            {
                return Enumerable.Empty<Module>();
            }

            // Obtener los módulos asignados al rol a través de los permisos
            var moduleIds = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p)
                .Where(p => p.Name == "Modules.View" && p.IsActive)
                .Join(_context.PermissionModules,
                    p => p.Id,
                    pm => pm.PermissionId,
                    (p, pm) => pm.ModuleId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Obtener los detalles de los módulos
            return await _dbSet
                .Where(m => moduleIds.Contains(m.Id) && m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Verifica si un rol tiene acceso a un módulo
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el rol tiene acceso al módulo, False en caso contrario</returns>
        public async Task<bool> RoleHasModuleAccessAsync(Guid roleId, Guid moduleId, CancellationToken cancellationToken = default)
        {
            // Verificar que el rol y el módulo existen
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId && r.IsActive, cancellationToken);
            var moduleExists = await _dbSet.AnyAsync(m => m.Id == moduleId && m.IsActive, cancellationToken);

            if (!roleExists || !moduleExists)
            {
                return false;
            }

            // Verificar si el rol tiene el permiso Modules.View para el módulo
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.IsActive)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p)
                .Where(p => p.Name == "Modules.View" && p.IsActive)
                .Join(_context.PermissionModules,
                    p => p.Id,
                    pm => pm.PermissionId,
                    (p, pm) => pm)
                .AnyAsync(pm => pm.ModuleId == moduleId, cancellationToken);
        }

        /// <summary>
        /// Asigna un módulo a un rol
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tarea completada</returns>
        public async Task AssignModuleToRoleAsync(Guid moduleId, Guid roleId, string userName, CancellationToken cancellationToken = default)
        {
            // Verificar que el rol y el módulo existen
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId && r.IsActive, cancellationToken);
            var module = await _dbSet.FirstOrDefaultAsync(m => m.Id == moduleId && m.IsActive, cancellationToken);

            if (role == null || module == null)
            {
                throw new InvalidOperationException("El rol o el módulo no existen o no están activos");
            }

            // Buscar el permiso Modules.View
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == "Modules.View" && p.IsActive, cancellationToken);

            if (permission == null)
            {
                throw new InvalidOperationException("El permiso Modules.View no existe o no está activo");
            }

            // Verificar si ya existe una relación entre el permiso y el módulo
            var permissionModule = await _context.PermissionModules
                .FirstOrDefaultAsync(pm => pm.PermissionId == permission.Id && pm.ModuleId == moduleId, cancellationToken);

            if (permissionModule == null)
            {
                // Crear la relación entre el permiso y el módulo
                permissionModule = new PermissionModule
                {
                    Id = Guid.NewGuid(),
                    PermissionId = permission.Id,
                    ModuleId = moduleId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = userName
                };

                await _context.PermissionModules.AddAsync(permissionModule, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // Verificar si el rol ya tiene asignado el permiso
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id, cancellationToken);

            if (rolePermission == null)
            {
                // Asignar el permiso al rol
                rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedBy = userName
                };

                await _context.RolePermissions.AddAsync(rolePermission, cancellationToken);
            }
            else if (!rolePermission.IsActive)
            {
                // Reactivar la asignación si estaba inactiva
                rolePermission.IsActive = true;
                rolePermission.LastModifiedAt = DateTime.UtcNow;
                rolePermission.LastModifiedBy = userName;
                _context.RolePermissions.Update(rolePermission);
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Revoca el acceso de un rol a un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tarea completada</returns>
        public async Task RevokeModuleFromRoleAsync(Guid moduleId, Guid roleId, CancellationToken cancellationToken = default)
        {
            // Buscar el permiso Modules.View
            var permission = await _context.Permissions
                .FirstOrDefaultAsync(p => p.Name == "Modules.View" && p.IsActive, cancellationToken);

            if (permission == null)
            {
                return;
            }

            // Verificar si existe una relación entre el permiso y el módulo
            var permissionModule = await _context.PermissionModules
                .FirstOrDefaultAsync(pm => pm.PermissionId == permission.Id && pm.ModuleId == moduleId, cancellationToken);

            if (permissionModule == null)
            {
                return;
            }

            // Buscar la asignación del permiso al rol
            var rolePermission = await _context.RolePermissions
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permission.Id && rp.IsActive, cancellationToken);

            if (rolePermission != null)
            {
                // Desactivar la asignación
                rolePermission.IsActive = false;
                rolePermission.LastModifiedAt = DateTime.UtcNow;
                rolePermission.LastModifiedBy = "System"; // Idealmente, debería recibir el nombre del usuario

                _context.RolePermissions.Update(rolePermission);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
