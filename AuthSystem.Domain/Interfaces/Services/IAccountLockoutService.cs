using System;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Interfaz para el servicio de bloqueo de cuentas
    /// </summary>
    public interface IAccountLockoutService
    {
        /// <summary>
        /// Registra un intento fallido de inicio de sesión
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si la cuenta ha sido bloqueada</returns>
        Task<bool> RecordFailedLoginAttemptAsync(Guid userId);

        /// <summary>
        /// Registra un inicio de sesión exitoso
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Task</returns>
        Task RecordSuccessfulLoginAsync(Guid userId);

        /// <summary>
        /// Verifica si una cuenta está bloqueada
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>True si la cuenta está bloqueada</returns>
        Task<bool> IsLockedOutAsync(Guid userId);

        /// <summary>
        /// Obtiene el tiempo restante de bloqueo
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Tiempo restante de bloqueo en segundos, o 0 si no está bloqueado</returns>
        Task<int> GetRemainingLockoutTimeAsync(Guid userId);

        /// <summary>
        /// Desbloquea una cuenta
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Task</returns>
        Task UnlockAccountAsync(Guid userId);
    }
}
