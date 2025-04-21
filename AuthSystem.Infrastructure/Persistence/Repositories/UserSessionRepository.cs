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
    /// Implementación del repositorio de sesiones de usuario
    /// </summary>
    public class UserSessionRepository : Repository<UserSession>, IUserSessionRepository
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de base de datos</param>
        public UserSessionRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtiene las sesiones de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="activeOnly">Indica si solo se deben obtener las sesiones activas</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de sesiones de usuario</returns>
        public async Task<IEnumerable<UserSession>> GetByUserAsync(Guid userId, bool activeOnly = true, CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Include(us => us.User)
                .Where(us => us.UserId == userId && us.IsActive);

            if (activeOnly)
            {
                query = query.Where(us => us.ExpiresAt > DateTime.UtcNow);
            }

            return await query.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Obtiene una sesión por su token de actualización
        /// </summary>
        /// <param name="refreshToken">Token de actualización</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Sesión de usuario encontrada o null</returns>
        public async Task<UserSession> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentException("El token de actualización no puede ser nulo o vacío", nameof(refreshToken));
            }

            return await _dbSet
                .Include(us => us.User)
                .FirstOrDefaultAsync(us => us.RefreshToken == refreshToken && us.IsActive && us.ExpiresAt > DateTime.UtcNow, cancellationToken);
        }

        /// <summary>
        /// Invalida todas las sesiones de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de sesiones invalidadas</returns>
        public async Task<int> InvalidateAllSessionsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var sessions = await _dbSet
                .Where(us => us.UserId == userId && us.IsActive && us.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            foreach (var session in sessions)
            {
                session.IsActive = false;
                session.LastModifiedAt = DateTime.UtcNow;
            }

            return sessions.Count;
        }

        /// <summary>
        /// Elimina las sesiones expiradas
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de sesiones eliminadas</returns>
        public async Task<int> DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var expiredSessions = await _dbSet
                .Where(us => us.ExpiresAt <= now && us.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var session in expiredSessions)
            {
                session.IsActive = false;
                session.LastModifiedAt = now;
            }

            return expiredSessions.Count;
        }

        /// <summary>
        /// Actualiza la última actividad de una sesión
        /// </summary>
        /// <param name="sessionId">ID de la sesión</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si se actualizó correctamente</returns>
        public async Task<bool> UpdateLastActivityAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            var session = await _dbSet.FirstOrDefaultAsync(us => us.Id == sessionId && us.IsActive, cancellationToken);
            if (session == null)
            {
                return false;
            }

            session.LastActivity = DateTime.UtcNow;
            session.LastModifiedAt = DateTime.UtcNow;

            return true;
        }
    }
}
