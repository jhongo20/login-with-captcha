using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthSystem.Infrastructure.Services
{
    /// <summary>
    /// Servicio para enviar notificaciones a los usuarios
    /// </summary>
    public class UserNotificationService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<UserNotificationService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailService">Servicio de correo electrónico</param>
        /// <param name="logger">Logger</param>
        public UserNotificationService(IEmailService emailService, ILogger<UserNotificationService> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Envía un correo electrónico de bienvenida al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendWelcomeEmailAsync(User user)
        {
            try
            {
                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Username", user.Username },
                    { "Email", user.Email },
                    { "CurrentDate", DateTime.Now.ToString("dd/MM/yyyy") }
                };

                return await _emailService.SendEmailAsync(
                    "UserCreated",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo de bienvenida al usuario: {user.Email}");
                return false;
            }
        }

        /// <summary>
        /// Envía un correo electrónico de actualización de cuenta al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendAccountUpdatedEmailAsync(User user)
        {
            try
            {
                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Username", user.Username },
                    { "Email", user.Email },
                    { "UpdateDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") }
                };

                return await _emailService.SendEmailAsync(
                    "UserUpdated",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo de actualización de cuenta al usuario: {user.Email}");
                return false;
            }
        }

        /// <summary>
        /// Envía un correo electrónico con código de activación al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="activationCode">Código de activación</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendActivationCodeEmailAsync(User user, string activationCode)
        {
            try
            {
                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Username", user.Username },
                    { "Email", user.Email },
                    { "ActivationCode", activationCode },
                    { "ExpirationTime", "24 horas" }
                };

                return await _emailService.SendEmailAsync(
                    "ActivationCode",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo con código de activación al usuario: {user.Email}");
                return false;
            }
        }

        /// <summary>
        /// Envía un correo electrónico de restablecimiento de contraseña al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="resetToken">Token de restablecimiento</param>
        /// <param name="resetUrl">URL de restablecimiento</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendPasswordResetEmailAsync(User user, string resetToken, string resetUrl)
        {
            try
            {
                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Username", user.Username },
                    { "Email", user.Email },
                    { "ResetToken", resetToken },
                    { "ResetUrl", resetUrl },
                    { "ExpirationTime", "1 hora" }
                };

                return await _emailService.SendEmailAsync(
                    "PasswordReset",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo de restablecimiento de contraseña al usuario: {user.Email}");
                return false;
            }
        }
    }
}
