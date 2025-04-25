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
                _logger.LogError(ex, "Error al enviar correo electrónico de actualización de cuenta al usuario: {Email}", user.Email);
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

        /// <summary>
        /// Envía un correo electrónico de suspensión de cuenta al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendAccountSuspendedEmailAsync(User user)
        {
            try
            {
                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Username", user.Username },
                    { "Email", user.Email },
                    { "CurrentDate", DateTime.Now.ToString("dd/MM/yyyy") },
                    { "SuspensionReason", "Decisión administrativa" }
                };

                return await _emailService.SendEmailAsync(
                    "AccountSuspended",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo electrónico de suspensión de cuenta al usuario: {Email}", user.Email);
                return false;
            }
        }

        /// <summary>
        /// Envía un correo electrónico de activación de cuenta al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendAccountActivatedEmailAsync(User user)
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
                    "AccountActivated",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo electrónico de activación de cuenta al usuario: {Email}", user.Email);
                return false;
            }
        }

        /// <summary>
        /// Envía una notificación de inicio de sesión al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="ipAddress">Dirección IP desde donde se realizó el inicio de sesión</param>
        /// <param name="userAgent">User-Agent del navegador</param>
        /// <param name="location">Ubicación geográfica aproximada (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendLoginNotificationAsync(User user, string ipAddress, string userAgent, string location = "Desconocida")
        {
            try
            {
                // Extraer información del dispositivo y navegador del User-Agent
                string device = "Dispositivo desconocido";
                string browser = "Navegador desconocido";

                if (!string.IsNullOrEmpty(userAgent))
                {
                    // Lógica simple para detectar dispositivo y navegador
                    if (userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone"))
                    {
                        device = "Dispositivo móvil";
                    }
                    else if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
                    {
                        device = "Tablet";
                    }
                    else
                    {
                        device = "Computadora";
                    }

                    // Detectar navegador
                    if (userAgent.Contains("Chrome"))
                    {
                        browser = "Google Chrome";
                    }
                    else if (userAgent.Contains("Firefox"))
                    {
                        browser = "Mozilla Firefox";
                    }
                    else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                    {
                        browser = "Safari";
                    }
                    else if (userAgent.Contains("Edge"))
                    {
                        browser = "Microsoft Edge";
                    }
                    else if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
                    {
                        browser = "Internet Explorer";
                    }
                }

                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "LoginDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                    { "IPAddress", ipAddress ?? "Desconocida" },
                    { "Location", location },
                    { "Device", device },
                    { "Browser", browser },
                    { "SecuritySettingsUrl", "/account/security" }, // Ajustar según la URL real
                    { "CurrentYear", DateTime.Now.Year.ToString() }
                };

                return await _emailService.SendEmailAsync(
                    "LoginNotification",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de inicio de sesión al usuario: {Email}", user.Email);
                return false;
            }
        }

        /// <summary>
        /// Envía una notificación de cambio de contraseña al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="ipAddress">Dirección IP desde donde se realizó el cambio</param>
        /// <param name="userAgent">User-Agent del navegador</param>
        /// <param name="location">Ubicación geográfica aproximada (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendPasswordChangedEmailAsync(User user, string ipAddress, string userAgent, string location = "Desconocida")
        {
            try
            {
                // Extraer información del dispositivo y navegador del User-Agent
                string device = "Dispositivo desconocido";
                string browser = "Navegador desconocido";

                if (!string.IsNullOrEmpty(userAgent))
                {
                    // Lógica simple para detectar dispositivo y navegador
                    if (userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone"))
                    {
                        device = "Dispositivo móvil";
                    }
                    else if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
                    {
                        device = "Tablet";
                    }
                    else
                    {
                        device = "Computadora";
                    }

                    // Detectar navegador
                    if (userAgent.Contains("Chrome"))
                    {
                        browser = "Google Chrome";
                    }
                    else if (userAgent.Contains("Firefox"))
                    {
                        browser = "Mozilla Firefox";
                    }
                    else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                    {
                        browser = "Safari";
                    }
                    else if (userAgent.Contains("Edge"))
                    {
                        browser = "Microsoft Edge";
                    }
                    else if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
                    {
                        browser = "Internet Explorer";
                    }
                }

                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Email", user.Email },
                    { "ChangeDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                    { "IPAddress", ipAddress ?? "Desconocida" },
                    { "Location", location },
                    { "Device", device },
                    { "Browser", browser },
                    { "SecuritySettingsUrl", "/account/security" }, // Ajustar según la URL real
                    { "SupportEmail", "soporte@authsystem.com" }, // Ajustar según el correo real de soporte
                    { "CurrentYear", DateTime.Now.Year.ToString() }
                };

                return await _emailService.SendEmailAsync(
                    "PasswordChanged",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de cambio de contraseña al usuario: {Email}", user.Email);
                return false;
            }
        }
    }
}
