using AuthSystem.API.Models.Requests;
using AuthSystem.API.Models.Responses;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AuthSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserNotificationService _userNotificationService;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(
            IUserRepository userRepository,
            UserNotificationService userNotificationService,
            ILogger<SecurityController> logger)
        {
            _userRepository = userRepository;
            _userNotificationService = userNotificationService;
            _logger = logger;
        }

        /// <summary>
        /// Reporta actividad inusual en la cuenta de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="request">Datos de la actividad inusual</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("{userId}/unusual-activity")]
        [Authorize(Roles = "Admin,SecurityOfficer")]
        public async Task<IActionResult> ReportUnusualActivity(Guid userId, [FromBody] ReportUnusualActivityRequest request)
        {
            try
            {
                // Verificar si el usuario existe
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ErrorResponse { Message = "Usuario no encontrado" });
                }

                // Obtener información del cliente que reporta la actividad
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
                string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Enviar notificación de actividad inusual
                await _userNotificationService.SendUnusualActivityEmailAsync(
                    user,
                    request.ActivityType,
                    request.IPAddress ?? ipAddress,
                    request.UserAgent ?? userAgent,
                    request.Location);

                // Registrar el evento en el log
                _logger.LogWarning(
                    "Actividad inusual reportada para el usuario {UserId}: {ActivityType} desde {IPAddress}",
                    userId, request.ActivityType, request.IPAddress ?? ipAddress);

                return Ok(new SuccessResponse { Message = "Notificación de actividad inusual enviada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reportar actividad inusual para el usuario {UserId}", userId);
                return StatusCode(500, new ErrorResponse { Message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Detecta y notifica automáticamente actividad inusual basada en patrones
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("{userId}/detect-unusual-activity")]
        [Authorize(Roles = "Admin,SecurityOfficer")]
        public async Task<IActionResult> DetectUnusualActivity(Guid userId)
        {
            try
            {
                // Verificar si el usuario existe
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new ErrorResponse { Message = "Usuario no encontrado" });
                }

                // Aquí iría la lógica para detectar actividad inusual basada en patrones
                // Por ejemplo, inicios de sesión desde ubicaciones inusuales, múltiples intentos fallidos, etc.
                
                // Para este ejemplo, simplemente simulamos que se detectó actividad inusual
                bool activityDetected = true;
                
                if (activityDetected)
                {
                    // Obtener información del cliente
                    string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Desconocida";
                    string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                    // Enviar notificación de actividad inusual
                    await _userNotificationService.SendUnusualActivityEmailAsync(
                        user,
                        "Múltiples inicios de sesión desde ubicaciones diferentes",
                        ipAddress,
                        userAgent);

                    return Ok(new SuccessResponse { 
                        Message = "Actividad inusual detectada y notificada", 
                        Data = new { ActivityDetected = true } 
                    });
                }

                return Ok(new SuccessResponse { 
                    Message = "No se detectó actividad inusual", 
                    Data = new { ActivityDetected = false } 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al detectar actividad inusual para el usuario {UserId}", userId);
                return StatusCode(500, new ErrorResponse { Message = "Error al procesar la solicitud" });
            }
        }

        /// <summary>
        /// Endpoint de prueba para enviar una notificación de actividad inusual
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>Resultado de la operación</returns>
        [HttpGet("test-unusual-activity/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> TestUnusualActivity(string email)
        {
            try
            {
                // Buscar usuario por email
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new ErrorResponse { Message = "Usuario no encontrado" });
                }

                // Obtener información del cliente
                string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "192.168.1.100";
                string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Enviar notificación de prueba
                await _userNotificationService.SendUnusualActivityEmailAsync(
                    user,
                    "Prueba de notificación de actividad inusual",
                    ipAddress,
                    userAgent,
                    "Ubicación de prueba");

                return Ok(new SuccessResponse { Message = "Notificación de prueba enviada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación de prueba");
                return StatusCode(500, new ErrorResponse { Message = "Error al procesar la solicitud" });
            }
        }
    }
}
