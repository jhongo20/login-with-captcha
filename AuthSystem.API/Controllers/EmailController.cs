using AuthSystem.Domain.Interfaces.Services;
using AuthSystem.Domain.Models.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para el envío de correos electrónicos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailService">Servicio de correo electrónico</param>
        /// <param name="logger">Logger</param>
        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Envía un correo electrónico utilizando una plantilla
        /// </summary>
        /// <param name="request">Datos para el envío del correo</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                // Validar la solicitud
                if (request == null)
                {
                    _logger.LogError("La solicitud es nula");
                    return BadRequest("La solicitud no puede ser nula");
                }

                if (string.IsNullOrEmpty(request.TemplateName))
                {
                    _logger.LogError("El nombre de la plantilla es nulo o vacío");
                    return BadRequest("El nombre de la plantilla es obligatorio");
                }

                if (string.IsNullOrEmpty(request.Email))
                {
                    _logger.LogError("El correo electrónico es nulo o vacío");
                    return BadRequest("El correo electrónico es obligatorio");
                }

                _logger.LogInformation($"Intentando enviar correo usando plantilla: {request.TemplateName} a: {request.Email}");
                
                if (request.TemplateData != null && request.TemplateData.Any())
                {
                    _logger.LogInformation($"Datos de la plantilla: {string.Join(", ", request.TemplateData.Select(kv => $"{kv.Key}={kv.Value}"))}");
                }
                else
                {
                    _logger.LogWarning("No se proporcionaron datos para la plantilla");
                }

                var result = await _emailService.SendEmailAsync(
                    request.TemplateName,
                    request.Email,
                    request.TemplateData ?? new Dictionary<string, string>(),
                    request.Attachments);

                if (result)
                {
                    _logger.LogInformation($"Correo enviado correctamente a {request.Email}");
                    return Ok(new { Message = "Correo electrónico enviado correctamente" });
                }
                else
                {
                    _logger.LogWarning($"Error al enviar correo a {request.Email}");
                    return BadRequest("Error al enviar el correo electrónico");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar el correo electrónico: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor al enviar el correo electrónico: {ex.Message}");
            }
        }

        /// <summary>
        /// Envía un correo electrónico a múltiples destinatarios utilizando una plantilla
        /// </summary>
        /// <param name="templateName">Nombre de la plantilla</param>
        /// <param name="emails">Lista de direcciones de correo electrónico</param>
        /// <param name="templateData">Datos para reemplazar en la plantilla</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("send-bulk")]
        public async Task<IActionResult> SendBulkEmail(
            [FromQuery] string templateName,
            [FromBody] List<string> emails,
            [FromQuery] Dictionary<string, string> templateData)
        {
            try
            {
                var result = await _emailService.SendBulkEmailAsync(
                    templateName,
                    emails,
                    templateData);

                if (result)
                {
                    return Ok(new { Message = "Correos electrónicos enviados correctamente" });
                }
                else
                {
                    return BadRequest("Error al enviar los correos electrónicos");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar los correos electrónicos masivos");
                return StatusCode(500, "Error interno del servidor al enviar los correos electrónicos masivos");
            }
        }

        /// <summary>
        /// Envía un correo electrónico personalizado sin utilizar una plantilla
        /// </summary>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="email">Dirección de correo electrónico del destinatario</param>
        /// <param name="htmlContent">Contenido HTML del correo</param>
        /// <param name="textContent">Contenido de texto plano del correo</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("send-custom")]
        public async Task<IActionResult> SendCustomEmail(
            [FromQuery] string subject,
            [FromQuery] string email,
            [FromBody] string htmlContent,
            [FromQuery] string textContent)
        {
            try
            {
                var result = await _emailService.SendCustomEmailAsync(
                    subject,
                    email,
                    htmlContent,
                    textContent);

                if (result)
                {
                    return Ok(new { Message = "Correo electrónico personalizado enviado correctamente" });
                }
                else
                {
                    return BadRequest("Error al enviar el correo electrónico personalizado");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo electrónico personalizado");
                return StatusCode(500, "Error interno del servidor al enviar el correo electrónico personalizado");
            }
        }
    }
}
