using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de restablecimiento de contraseña
    /// </summary>
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PasswordResetService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepository">Repositorio de usuarios</param>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="configuration">Configuración</param>
        /// <param name="logger">Logger</param>
        public PasswordResetService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<PasswordResetService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Genera un token de restablecimiento de contraseña para un usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <returns>Token de restablecimiento</returns>
        public async Task<string> GenerateResetTokenAsync(User user)
        {
            try
            {
                // Generar un token aleatorio
                var tokenBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(tokenBytes);
                }
                var token = Convert.ToBase64String(tokenBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");

                // Establecer la fecha de expiración (1 hora)
                var expirationTime = DateTime.UtcNow.AddHours(1);

                // Guardar el token y la fecha de expiración en el usuario
                user.PasswordResetToken = token;
                user.PasswordResetTokenExpiry = expirationTime;

                // Actualizar el usuario en la base de datos
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar token de restablecimiento de contraseña para el usuario: {UserId}", user.Id);
                throw;
            }
        }

        /// <summary>
        /// Valida un token de restablecimiento de contraseña
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="token">Token de restablecimiento</param>
        /// <returns>True si el token es válido, False en caso contrario</returns>
        public async Task<bool> ValidateResetTokenAsync(User user, string token)
        {
            try
            {
                // Verificar que el usuario tenga un token de restablecimiento
                if (string.IsNullOrEmpty(user.PasswordResetToken) || user.PasswordResetTokenExpiry == null)
                {
                    _logger.LogWarning("El usuario {UserId} no tiene un token de restablecimiento", user.Id);
                    return false;
                }

                // Verificar que el token no haya expirado
                if (user.PasswordResetTokenExpiry < DateTime.UtcNow)
                {
                    _logger.LogWarning("El token de restablecimiento para el usuario {UserId} ha expirado", user.Id);
                    return false;
                }

                // Verificar que el token coincida
                if (user.PasswordResetToken != token)
                {
                    _logger.LogWarning("Token de restablecimiento inválido para el usuario {UserId}", user.Id);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar token de restablecimiento de contraseña para el usuario: {UserId}", user.Id);
                return false;
            }
        }

        /// <summary>
        /// Restablece la contraseña de un usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="token">Token de restablecimiento</param>
        /// <param name="newPassword">Nueva contraseña</param>
        /// <returns>True si la contraseña se restableció correctamente, False en caso contrario</returns>
        public async Task<bool> ResetPasswordAsync(User user, string token, string newPassword)
        {
            try
            {
                // Validar el token
                if (!await ValidateResetTokenAsync(user, token))
                {
                    return false;
                }

                // Actualizar la contraseña del usuario
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                
                // Limpiar el token de restablecimiento
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;
                
                // Actualizar la fecha del último cambio de contraseña
                user.LastPasswordChangeAt = DateTime.UtcNow;

                // Actualizar el usuario en la base de datos
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restablecer la contraseña para el usuario: {UserId}", user.Id);
                return false;
            }
        }
    }
}
