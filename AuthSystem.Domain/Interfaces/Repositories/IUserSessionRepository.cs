using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de sesiones de usuario
    /// </summary>
    public interface IUserSessionRepository : IRepository<UserSession>
    {
        /// <summary>
        /// Obtiene las sesiones de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="activeOnly">Indica si solo se deben obtener las sesiones activas</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de sesiones de usuario</returns>
        Task<IEnumerable<UserSession>> GetByUserAsync(Guid userId, bool activeOnly = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene una sesión por su token de actualización
        /// </summary>
        /// <param name="refreshToken">Token de actualización</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Sesión de usuario encontrada o null</returns>
        Task<UserSession> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Invalida todas las sesiones de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de sesiones invalidadas</returns>
        Task<int> InvalidateAllSessionsAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Elimina las sesiones expiradas
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de sesiones eliminadas</returns>
        Task<int> DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza la última actividad de una sesión
        /// </summary>
        /// <param name="sessionId">ID de la sesión</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> UpdateLastActivityAsync(Guid sessionId, CancellationToken cancellationToken = default);
    }
}
