using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Interfaz para el servicio de tokens JWT
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Genera un token JWT para un usuario
        /// </summary>
        /// <param name="user">Usuario para el que se genera el token</param>
        /// <returns>Token JWT</returns>
        string GenerateToken(User user);

        /// <summary>
        /// Genera un token JWT
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="email">Correo electrónico</param>
        /// <param name="roles">Roles del usuario</param>
        /// <param name="permissions">Permisos del usuario</param>
        /// <param name="additionalClaims">Reclamaciones adicionales (opcional)</param>
        /// <param name="expirationMinutes">Minutos de expiración (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Token JWT generado</returns>
        Task<string> GenerateTokenAsync(
            Guid userId,
            string username,
            string email,
            IEnumerable<string> roles,
            IEnumerable<string> permissions,
            IDictionary<string, string> additionalClaims = null,
            int expirationMinutes = 60,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Genera un token de actualización
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Token de actualización generado</returns>
        Task<string> GenerateRefreshTokenAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida un token de actualización
        /// </summary>
        /// <param name="refreshToken">Token de actualización</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tupla con (esValido, idUsuario)</returns>
        Task<(bool isValid, Guid userId)> ValidateRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="validateLifetime">Indica si se debe validar la vida útil</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el token es válido</returns>
        Task<bool> ValidateTokenAsync(
            string token,
            bool validateLifetime = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene las reclamaciones de un token JWT
        /// </summary>
        /// <param name="token">Token JWT</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Reclamaciones del token</returns>
        Task<ClaimsPrincipal> GetPrincipalFromTokenAsync(
            string token,
            CancellationToken cancellationToken = default);
    }
}
