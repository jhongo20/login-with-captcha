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
    /// Implementación del repositorio de roles
    /// </summary>
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public RoleRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene un rol por su nombre
        /// </summary>
        /// <param name="name">Nombre del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Rol encontrado o null</returns>
        public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("El nombre del rol no puede ser nulo o vacío", nameof(name));
            }

            return await _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Name == name && r.IsActive, cancellationToken);
        }

        /// <summary>
        /// Verifica si un nombre de rol ya existe
        /// </summary>
        /// <param name="name">Nombre del rol</param>
        /// <param name="excludeRoleId">ID de rol a excluir (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el nombre de rol ya existe</returns>
        public async Task<bool> NameExistsAsync(string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("El nombre del rol no puede ser nulo o vacío", nameof(name));
            }

            var query = _dbSet.Where(r => r.Name == name && r.IsActive);

            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene los roles de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de roles del usuario</returns>
        public async Task<IEnumerable<Role>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => r.UserRoles.Any(ur => ur.UserId == userId && ur.IsActive) && r.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad encontrada o null</returns>
        public override async Task<Role> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id && r.IsActive, cancellationToken);
        }
    }
}
