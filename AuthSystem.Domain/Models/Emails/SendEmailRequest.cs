using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Emails
{
    /// <summary>
    /// Modelo para la solicitud de envío de un correo electrónico
    /// </summary>
    public class SendEmailRequest
    {
        /// <summary>
        /// Nombre de la plantilla a utilizar
        /// </summary>
        [Required(ErrorMessage = "El nombre de la plantilla es obligatorio")]
        public string TemplateName { get; set; }
        
        /// <summary>
        /// Dirección de correo electrónico del destinatario
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico del destinatario es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; }
        
        /// <summary>
        /// Datos para reemplazar en la plantilla (clave-valor)
        /// </summary>
        public Dictionary<string, string> TemplateData { get; set; } = new Dictionary<string, string>();
        
        /// <summary>
        /// Archivos adjuntos (rutas a los archivos)
        /// </summary>
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
