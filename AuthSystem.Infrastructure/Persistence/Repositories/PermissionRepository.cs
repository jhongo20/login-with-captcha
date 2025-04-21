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
    /// Implementación del repositorio de permisos
    /// </summary>
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene un permiso por su nombre
        /// </summary>
        /// <param name="name">Nombre del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Permiso encontrado o null</returns>
        public async Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("El nombre del permiso no puede ser nulo o vacío", nameof(name));
            }

            return await _dbSet.FirstOrDefaultAsync(p => p.Name == name && p.IsActive, cancellationToken);
        }

        /// <summary>
        /// Verifica si un nombre de permiso ya existe
        /// </summary>
        /// <param name="name">Nombre del permiso</param>
        /// <param name="excludePermissionId">ID de permiso a excluir (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el nombre de permiso ya existe</returns>
        public async Task<bool> NameExistsAsync(string name, Guid? excludePermissionId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("El nombre del permiso no puede ser nulo o vacío", nameof(name));
            }

            var query = _dbSet.Where(p => p.Name == name && p.IsActive);

            if (excludePermissionId.HasValue)
            {
                query = query.Where(p => p.Id != excludePermissionId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene los permisos de un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos del rol</returns>
        public async Task<IEnumerable<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(p => p.RolePermissions.Any(rp => rp.RoleId == roleId && rp.IsActive) && p.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene los permisos de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos del usuario</returns>
        public async Task<IEnumerable<Permission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _context.Permissions
                .Where(p => p.RolePermissions.Any(rp => 
                    rp.Role.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive) && 
                    rp.IsActive) && 
                    p.IsActive)
                .ToListAsync(cancellationToken);
        }
    }
}
