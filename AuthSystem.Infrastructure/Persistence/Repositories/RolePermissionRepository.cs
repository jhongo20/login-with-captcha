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
    /// Implementación del repositorio de permisos de rol
    /// </summary>
    public class RolePermissionRepository : Repository<RolePermission>, IRolePermissionRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public RolePermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene los permisos de un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos de rol</returns>
        public async Task<IEnumerable<RolePermission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId && rp.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene los roles que tienen un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos de rol</returns>
        public async Task<IEnumerable<RolePermission>> GetByPermissionAsync(Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rp => rp.Role)
                .Where(rp => rp.PermissionId == permissionId && rp.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un permiso de rol específico
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Permiso de rol encontrado o null</returns>
        public async Task<RolePermission> GetByRoleAndPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.IsActive, cancellationToken);
        }

        /// <summary>
        /// Verifica si un rol tiene un permiso específico
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el rol tiene el permiso</returns>
        public async Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId && rp.IsActive, cancellationToken);
        }
    }
}
