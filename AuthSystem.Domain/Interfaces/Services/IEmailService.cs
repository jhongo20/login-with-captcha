using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Interfaz para el servicio de correo electrónico
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico utilizando una plantilla
        /// </summary>
        /// <param name="templateName">Nombre de la plantilla</param>
        /// <param name="email">Dirección de correo electrónico del destinatario</param>
        /// <param name="templateData">Datos para reemplazar en la plantilla</param>
        /// <param name="attachments">Archivos adjuntos (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        Task<bool> SendEmailAsync(string templateName, string email, Dictionary<string, string> templateData, List<string>? attachments = null);
        
        /// <summary>
        /// Envía un correo electrónico personalizado sin utilizar una plantilla
        /// </summary>
        /// <param name="subject">Asunto del correo</param>
        /// <param name="email">Dirección de correo electrónico del destinatario</param>
        /// <param name="htmlContent">Contenido HTML del correo</param>
        /// <param name="textContent">Contenido de texto plano del correo</param>
        /// <param name="attachments">Archivos adjuntos (opcional)</param>
        /// <returns>True si el correo se envió correctamente, False en caso contrario</returns>
        Task<bool> SendCustomEmailAsync(string subject, string email, string htmlContent, string textContent, List<string>? attachments = null);
        
        /// <summary>
        /// Envía un correo electrónico a múltiples destinatarios utilizando una plantilla
        /// </summary>
        /// <param name="templateName">Nombre de la plantilla</param>
        /// <param name="emails">Lista de direcciones de correo electrónico de los destinatarios</param>
        /// <param name="templateData">Datos para reemplazar en la plantilla</param>
        /// <param name="attachments">Archivos adjuntos (opcional)</param>
        /// <returns>True si el correo se envió correctamente a todos los destinatarios, False en caso contrario</returns>
        Task<bool> SendBulkEmailAsync(string templateName, List<string> emails, Dictionary<string, string> templateData, List<string>? attachments = null);
    }
}
