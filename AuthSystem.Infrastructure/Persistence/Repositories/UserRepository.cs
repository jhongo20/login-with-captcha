using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Common.Enums;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de usuarios
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("El nombre de usuario no puede ser nulo o vacío", nameof(username));
            }

            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.UserStatus == UserStatus.Active, cancellationToken);
        }

        /// <summary>
        /// Obtiene un usuario por su correo electrónico
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("El correo electrónico no puede ser nulo o vacío", nameof(email));
            }

            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.UserStatus == UserStatus.Active, cancellationToken);
        }

        /// <summary>
        /// Verifica si un nombre de usuario ya existe
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="excludeUserId">ID de usuario a excluir (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el nombre de usuario ya existe</returns>
        public async Task<bool> UsernameExistsAsync(string username, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("El nombre de usuario no puede ser nulo o vacío", nameof(username));
            }

            var query = _dbSet.Where(u => u.Username == username && u.UserStatus != UserStatus.Deleted);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Verifica si un correo electrónico ya existe
        /// </summary>
        /// <param name="email">Correo electrónico</param>
        /// <param name="excludeUserId">ID de usuario a excluir (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el correo electrónico ya existe</returns>
        public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("El correo electrónico no puede ser nulo o vacío", nameof(email));
            }

            var query = _dbSet.Where(u => u.Email == email && u.UserStatus != UserStatus.Deleted);

            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene los usuarios por rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de usuarios con el rol especificado</returns>
        public async Task<IEnumerable<User>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId && ur.IsActive) && u.UserStatus == UserStatus.Active)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene una entidad por su ID
        /// </summary>
        /// <param name="id">ID de la entidad</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Entidad encontrada o null</returns>
        public override async Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id && u.UserStatus == UserStatus.Active, cancellationToken);
        }

        /// <summary>
        /// Obtiene un usuario por su ID incluyendo inactivos
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Usuario encontrado o null</returns>
        public async Task<User> GetByIdIncludingInactiveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de todos los usuarios activos</returns>
        public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserStatus == UserStatus.Active)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene todos los usuarios incluyendo los inactivos
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de todos los usuarios, independientemente de su estado</returns>
        public async Task<IEnumerable<User>> GetAllUsersIncludingInactiveAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene usuarios por su estado
        /// </summary>
        /// <param name="status">Estado del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de usuarios con el estado especificado</returns>
        public async Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserStatus == status)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Actualiza el estado de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="status">Nuevo estado</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si la actualización fue exitosa</returns>
        public async Task<bool> UpdateUserStatusAsync(Guid userId, UserStatus status, CancellationToken cancellationToken = default)
        {
            var user = await _dbSet.FindAsync(new object[] { userId }, cancellationToken);
            if (user == null)
            {
                return false;
            }

            user.UserStatus = status;
            user.LastModifiedAt = DateTime.UtcNow;
            
            _dbSet.Update(user);
            return true;
        }
    }
}
