using System;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Interfaz para el servicio de restablecimiento de contraseña
    /// </summary>
    public interface IPasswordResetService
    {
        /// <summary>
        /// Genera un token de restablecimiento de contraseña para un usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <returns>Token de restablecimiento</returns>
        Task<string> GenerateResetTokenAsync(User user);

        /// <summary>
        /// Valida un token de restablecimiento de contraseña
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="token">Token de restablecimiento</param>
        /// <returns>True si el token es válido, False en caso contrario</returns>
        Task<bool> ValidateResetTokenAsync(User user, string token);

        /// <summary>
        /// Restablece la contraseña de un usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="token">Token de restablecimiento</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>True si la contraseña se restableció correctamente, False en caso contrario</returns>
        Task<bool> ResetPasswordAsync(User user, string token, string newPassword);
    }
}
