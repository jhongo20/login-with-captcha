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
                string device = DetermineDevice(userAgent);
                string browser = DetermineBrowser(userAgent);
                
                // Obtener ubicación aproximada si no se proporcionó
                if (location == "Desconocida" && !string.IsNullOrEmpty(ipAddress))
                {
                    location = await GetLocationFromIpAsync(ipAddress);
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
                string device = DetermineDevice(userAgent);
                string browser = DetermineBrowser(userAgent);
                
                // Obtener ubicación aproximada si no se proporcionó
                if (location == "Desconocida" && !string.IsNullOrEmpty(ipAddress))
                {
                    location = await GetLocationFromIpAsync(ipAddress);
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

        /// <summary>
        /// Envía una notificación de cuenta bloqueada al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="lockoutEnd">Fecha y hora de finalización del bloqueo</param>
        /// <param name="ipAddress">Dirección IP desde donde se realizaron los intentos</param>
        /// <param name="userAgent">User-Agent del navegador</param>
        /// <param name="location">Ubicación geográfica aproximada (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendAccountLockedEmailAsync(User user, DateTime lockoutEnd, string ipAddress, string userAgent, string location = "Desconocida")
        {
            try
            {
                // Extraer información del dispositivo y navegador del User-Agent
                string device = DetermineDevice(userAgent);
                string browser = DetermineBrowser(userAgent);
                
                // Obtener ubicación aproximada si no se proporcionó
                if (location == "Desconocida" && !string.IsNullOrEmpty(ipAddress))
                {
                    location = await GetLocationFromIpAsync(ipAddress);
                }

                // Calcular la duración del bloqueo
                TimeSpan lockoutDuration = lockoutEnd - DateTime.Now;
                string formattedDuration = FormatTimeSpan(lockoutDuration);

                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Email", user.Email },
                    { "LockoutDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") },
                    { "LockoutDuration", formattedDuration },
                    { "UnlockDate", lockoutEnd.ToString("dd/MM/yyyy HH:mm:ss") },
                    { "IPAddress", ipAddress ?? "Desconocida" },
                    { "Location", location },
                    { "Device", device },
                    { "Browser", browser },
                    { "ContactSupportUrl", "/contact-support" }, // Ajustar según la URL real
                    { "SupportEmail", "soporte@authsystem.com" }, // Ajustar según el correo real de soporte
                    { "CurrentYear", DateTime.Now.Year.ToString() }
                };

                return await _emailService.SendEmailAsync(
                    "AccountLocked",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de cuenta bloqueada al usuario: {Email}", user.Email);
                return false;
            }
        }

        /// <summary>
        /// Envía una notificación de actividad inusual al usuario
        /// </summary>
        /// <param name="user">Usuario</param>
        /// <param name="activityType">Tipo de actividad inusual detectada</param>
        /// <param name="ipAddress">Dirección IP desde donde se realizó la actividad</param>
        /// <param name="userAgent">User-Agent del navegador</param>
        /// <param name="location">Ubicación geográfica aproximada (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendUnusualActivityEmailAsync(User user, string activityType, string ipAddress, string userAgent, string location = "Desconocida")
        {
            try
            {
                // Extraer información del dispositivo y navegador del User-Agent
                string device = DetermineDevice(userAgent);
                string browser = DetermineBrowser(userAgent);
                
                // Obtener ubicación aproximada si no se proporcionó
                if (location == "Desconocida" && !string.IsNullOrEmpty(ipAddress))
                {
                    location = await GetLocationFromIpAsync(ipAddress);
                }

                // Fecha y hora actual
                DateTime now = DateTime.Now;
                string activityDate = now.ToString("dd/MM/yyyy");
                string activityTime = now.ToString("HH:mm:ss");

                var templateData = new Dictionary<string, string>
                {
                    { "FullName", user.FullName },
                    { "Email", user.Email },
                    { "ActivityType", activityType },
                    { "ActivityDate", activityDate },
                    { "ActivityTime", activityTime },
                    { "IPAddress", ipAddress ?? "Desconocida" },
                    { "Location", location },
                    { "Device", device },
                    { "Browser", browser },
                    { "SecuritySettingsUrl", "/account/security" }, // Ajustar según la URL real
                    { "SupportEmail", "soporte@authsystem.com" }, // Ajustar según el correo real de soporte
                    { "CurrentYear", now.Year.ToString() }
                };

                return await _emailService.SendEmailAsync(
                    "UnusualActivity",
                    user.Email,
                    templateData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de actividad inusual al usuario: {Email}", user.Email);
                return false;
            }
        }

        /// <summary>
        /// Determina el tipo de dispositivo a partir del User-Agent
        /// </summary>
        /// <param name="userAgent">User-Agent del navegador</param>
        /// <returns>Tipo de dispositivo</returns>
        private string DetermineDevice(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Dispositivo desconocido";

            if (userAgent.Contains("Mobile") || userAgent.Contains("Android") || userAgent.Contains("iPhone"))
                return "Dispositivo móvil";
            else if (userAgent.Contains("Tablet") || userAgent.Contains("iPad"))
                return "Tablet";
            else
                return "Computadora";
        }

        /// <summary>
        /// Determina el navegador a partir del User-Agent
        /// </summary>
        /// <param name="userAgent">User-Agent del navegador</param>
        /// <returns>Nombre del navegador</returns>
        private string DetermineBrowser(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return "Navegador desconocido";

            if (userAgent.Contains("Chrome") && !userAgent.Contains("Edg"))
                return "Google Chrome";
            else if (userAgent.Contains("Firefox"))
                return "Mozilla Firefox";
            else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
                return "Safari";
            else if (userAgent.Contains("Edg"))
                return "Microsoft Edge";
            else if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
                return "Internet Explorer";
            else
                return "Navegador desconocido";
        }

        /// <summary>
        /// Obtiene la ubicación aproximada a partir de una dirección IP
        /// </summary>
        /// <param name="ipAddress">Dirección IP</param>
        /// <returns>Ubicación aproximada</returns>
        private async Task<string> GetLocationFromIpAsync(string ipAddress)
        {
            // Implementación básica - en un entorno real, se utilizaría un servicio de geolocalización
            // como MaxMind GeoIP, ipstack, ipinfo.io, etc.
            return "Desconocida";
        }

        /// <summary>
        /// Formatea un TimeSpan en un formato legible
        /// </summary>
        /// <param name="timeSpan">TimeSpan a formatear</param>
        /// <returns>Formato legible</returns>
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 60)
                return $"{Math.Ceiling(timeSpan.TotalMinutes)} minutos";
            else if (timeSpan.TotalHours < 24)
                return $"{Math.Ceiling(timeSpan.TotalHours)} horas";
            else
                return $"{Math.Ceiling(timeSpan.TotalDays)} días";
        }
    }
}
