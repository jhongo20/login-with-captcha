using AuthSystem.Domain.Interfaces.Repositories;
using AuthSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AuthSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de correo electrónico utilizando MailKit
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="emailTemplateRepository">Repositorio de plantillas de correo electrónico</param>
        /// <param name="configuration">Configuración de la aplicación</param>
        /// <param name="logger">Logger</param>
        public EmailService(
            IEmailTemplateRepository emailTemplateRepository,
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            _emailTemplateRepository = emailTemplateRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Envía un correo electrónico utilizando una plantilla
        /// </summary>
        /// <param name="templateName">Nombre de la plantilla</param>
        /// <param name="email">Dirección de correo electrónico del destinatario</param>
        /// <param name="templateData">Datos para reemplazar en la plantilla</param>
        /// <param name="attachments">Archivos adjuntos (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendEmailAsync(string templateName, string email, Dictionary<string, string> templateData, List<string>? attachments = null)
        {
            try
            {
                // Obtener la plantilla de correo electrónico
                var template = await _emailTemplateRepository.GetByNameAsync(templateName);
                if (template == null)
                {
                    _logger.LogError($"No se encontró la plantilla de correo electrónico con nombre: {templateName}");
                    return false;
                }

                // Reemplazar variables en el asunto y contenido
                string subject = template.Subject;
                string htmlContent = template.HtmlContent;
                string textContent = template.TextContent;

                // Reemplazar variables en el asunto y contenido
                foreach (var item in templateData)
                {
                    subject = subject.Replace($"{{{{{item.Key}}}}}", item.Value);
                    htmlContent = htmlContent.Replace($"{{{{{item.Key}}}}}", item.Value);
                    textContent = textContent.Replace($"{{{{{item.Key}}}}}", item.Value);
                }

                // Enviar el correo electrónico
                return await SendEmailInternalAsync(subject, email, htmlContent, textContent, attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo electrónico con plantilla {templateName} a {email}");
                return false;
            }
        }

        /// <summary>
        /// Envía un correo electrónico personalizado sin utilizar una plantilla
        /// </summary>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="email">Dirección de correo electrónico del destinatario</param>
        /// <param name="htmlContent">Contenido HTML del correo</param>
        /// <param name="textContent">Contenido de texto plano del correo</param>
        /// <param name="attachments">Archivos adjuntos (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        public async Task<bool> SendCustomEmailAsync(string subject, string email, string htmlContent, string textContent, List<string>? attachments = null)
        {
            try
            {
                return await SendEmailInternalAsync(subject, email, htmlContent, textContent, attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo electrónico personalizado a {email}");
                return false;
            }
        }

        /// <summary>
        /// Envía un correo electrónico a múltiples destinatarios utilizando una plantilla
        /// </summary>
        /// <param name="templateName">Nombre de la plantilla</param>
        /// <param name="emails">Lista de direcciones de correo electrónico de los destinatarios</param>
        /// <param name="templateData">Datos para reemplazar en la plantilla</param>
        /// <param name="attachments">Archivos adjuntos (opcional)</param>
        /// <returns>True si el correo se envió correctamente a todos los destinatarios, False en caso contrario</returns>
        public async Task<bool> SendBulkEmailAsync(string templateName, List<string> emails, Dictionary<string, string> templateData, List<string>? attachments = null)
        {
            try
            {
                // Obtener la plantilla de correo electrónico
                var template = await _emailTemplateRepository.GetByNameAsync(templateName);
                if (template == null)
                {
                    _logger.LogError($"No se encontró la plantilla de correo electrónico con nombre: {templateName}");
                    return false;
                }

                // Reemplazar variables en el asunto y contenido
                string subject = template.Subject;
                string htmlContent = template.HtmlContent;
                string textContent = template.TextContent;

                // Reemplazar variables en el asunto y contenido
                foreach (var item in templateData)
                {
                    subject = subject.Replace($"{{{{{item.Key}}}}}", item.Value);
                    htmlContent = htmlContent.Replace($"{{{{{item.Key}}}}}", item.Value);
                    textContent = textContent.Replace($"{{{{{item.Key}}}}}", item.Value);
                }

                // Enviar el correo electrónico a cada destinatario
                bool allSuccess = true;
                foreach (var email in emails)
                {
                    bool success = await SendEmailInternalAsync(subject, email, htmlContent, textContent, attachments);
                    if (!success)
                    {
                        allSuccess = false;
                        _logger.LogWarning($"Error al enviar correo electrónico a {email}");
                    }
                }

                return allSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo electrónico masivo con plantilla {templateName}");
                return false;
            }
        }

        /// <summary>
        /// Método interno para enviar un correo electrónico
        /// </summary>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="email">Dirección de correo electrónico del destinatario</param>
        /// <param name="htmlContent">Contenido HTML del correo</param>
        /// <param name="textContent">Contenido de texto plano del correo</param>
        /// <param name="attachments">Archivos adjuntos (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        private async Task<bool> SendEmailInternalAsync(string subject, string email, string htmlContent, string textContent, List<string>? attachments = null)
        {
            try
            {
                // Crear el mensaje
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_configuration["Email:SenderName"], _configuration["Email:SenderEmail"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                // Crear el cuerpo del mensaje
                var builder = new BodyBuilder();
                
                // Asegurarse de que siempre haya contenido HTML
                if (string.IsNullOrWhiteSpace(htmlContent))
                {
                    // Si no hay contenido HTML, convertir el texto plano a HTML básico
                    htmlContent = $"<html><body><pre>{textContent}</pre></body></html>";
                }
                
                builder.HtmlBody = htmlContent;
                builder.TextBody = textContent;

                // Añadir archivos adjuntos
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        if (File.Exists(attachment))
                        {
                            builder.Attachments.Add(attachment);
                        }
                        else
                        {
                            _logger.LogWarning($"No se encontró el archivo adjunto: {attachment}");
                        }
                    }
                }

                message.Body = builder.ToMessageBody();

                // Enviar el mensaje
                using (var client = new SmtpClient())
                {
                    try
                    {
                        _logger.LogInformation($"Intentando conectar al servidor SMTP: {_configuration["Email:SmtpServer"]}:{_configuration["Email:SmtpPort"]}");
                        
                        // Configurar opciones de seguridad para Gmail
                        var secureSocketOptions = SecureSocketOptions.StartTls;
                        if (_configuration["Email:UseSsl"].ToLower() == "true")
                        {
                            secureSocketOptions = SecureSocketOptions.StartTls; // Usar StartTls para Gmail en lugar de SslOnConnect
                        }
                        
                        // Conectar al servidor SMTP
                        await client.ConnectAsync(
                            _configuration["Email:SmtpServer"],
                            int.Parse(_configuration["Email:SmtpPort"]),
                            secureSocketOptions);

                        _logger.LogInformation("Conexión al servidor SMTP exitosa. Intentando autenticar...");
                        
                        // Autenticar si es necesario
                        if (!string.IsNullOrEmpty(_configuration["Email:Username"]) && !string.IsNullOrEmpty(_configuration["Email:Password"]))
                        {
                            await client.AuthenticateAsync(_configuration["Email:Username"], _configuration["Email:Password"]);
                            _logger.LogInformation("Autenticación exitosa.");
                        }

                        _logger.LogInformation("Enviando mensaje...");
                        
                        // Enviar el mensaje
                        await client.SendAsync(message);

                        _logger.LogInformation("Mensaje enviado. Desconectando...");
                        
                        // Desconectar
                        await client.DisconnectAsync(true);
                        
                        _logger.LogInformation($"Correo electrónico enviado correctamente a {email}");
                    }
                    catch (Exception smtpEx)
                    {
                        _logger.LogError(smtpEx, $"Error específico de SMTP al enviar correo electrónico a {email}");
                        throw; // Re-lanzar la excepción para que sea capturada por el bloque catch exterior
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo electrónico a {email}");
                return false;
            }
        }
    }
}
