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
    /// Implementación del repositorio de roles de usuario
    /// </summary>
    public class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public UserRoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene los roles de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de roles de usuario</returns>
        public async Task<IEnumerable<UserRole>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene los usuarios de un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de roles de usuario</returns>
        public async Task<IEnumerable<UserRole>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(ur => ur.User)
                .Where(ur => ur.RoleId == roleId && ur.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene un rol de usuario específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Rol de usuario encontrado o null</returns>
        public async Task<UserRole> GetByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive, cancellationToken);
        }

        /// <summary>
        /// Verifica si un usuario tiene un rol específico
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el usuario tiene el rol</returns>
        public async Task<bool> UserHasRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.IsActive, cancellationToken);
        }
    }
}
