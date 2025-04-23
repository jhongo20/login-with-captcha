using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de PermissionModule
    /// </summary>
    public class PermissionModuleRepository : IPermissionModuleRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public PermissionModuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los permisos de un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>Lista de permisos</returns>
        public async Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(Guid moduleId)
        {
            return await _context.PermissionModules
                .Where(pm => pm.ModuleId == moduleId)
                .Include(pm => pm.Permission)
                .Select(pm => pm.Permission)
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los módulos que requieren un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Lista de módulos</returns>
        public async Task<IEnumerable<Module>> GetModulesByPermissionAsync(Guid permissionId)
        {
            return await _context.PermissionModules
                .Where(pm => pm.PermissionId == permissionId)
                .Include(pm => pm.Module)
                .Select(pm => pm.Module)
                .Where(m => m.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Asigna un permiso a un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <returns>Task</returns>
        public async Task AssignPermissionToModuleAsync(Guid moduleId, Guid permissionId, string userName)
        {
            // Verificar que el módulo y el permiso existen
            var module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == moduleId && m.IsActive);
            var permission = await _context.Permissions.FirstOrDefaultAsync(p => p.Id == permissionId && p.IsActive);

            if (module == null)
            {
                throw new InvalidOperationException($"El módulo con ID {moduleId} no existe o no está activo");
            }

            if (permission == null)
            {
                throw new InvalidOperationException($"El permiso con ID {permissionId} no existe o no está activo");
            }

            // Verificar si ya existe la relación
            var existingRelation = await _context.PermissionModules
                .FirstOrDefaultAsync(pm => pm.ModuleId == moduleId && pm.PermissionId == permissionId);

            if (existingRelation != null)
            {
                throw new InvalidOperationException($"El permiso ya está asignado al módulo");
            }

            // Crear la relación
            var permissionModule = new PermissionModule
            {
                Id = Guid.NewGuid(),
                ModuleId = moduleId,
                PermissionId = permissionId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userName,
                LastModifiedAt = DateTime.UtcNow,
                LastModifiedBy = userName
            };

            await _context.PermissionModules.AddAsync(permissionModule);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Revoca un permiso de un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Task</returns>
        public async Task RevokePermissionFromModuleAsync(Guid moduleId, Guid permissionId)
        {
            // Buscar la relación
            var permissionModule = await _context.PermissionModules
                .FirstOrDefaultAsync(pm => pm.ModuleId == moduleId && pm.PermissionId == permissionId);

            if (permissionModule == null)
            {
                throw new InvalidOperationException($"El permiso no está asignado al módulo");
            }

            // Eliminar la relación
            _context.PermissionModules.Remove(permissionModule);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifica si un módulo requiere un permiso específico
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>True si el módulo requiere el permiso, False en caso contrario</returns>
        public async Task<bool> ModuleRequiresPermissionAsync(Guid moduleId, Guid permissionId)
        {
            return await _context.PermissionModules
                .AnyAsync(pm => pm.ModuleId == moduleId && pm.PermissionId == permissionId);
        }
    }
}
