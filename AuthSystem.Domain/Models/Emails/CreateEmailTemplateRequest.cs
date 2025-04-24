using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Emails
{
    /// <summary>
    /// Modelo para la solicitud de creación de una plantilla de correo electrónico
    /// </summary>
    public class CreateEmailTemplateRequest
    {
        /// <summary>
        /// Nombre único de la plantilla (ej: "UserCreated", "PasswordReset")
        /// </summary>
        [Required(ErrorMessage = "El nombre de la plantilla es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Name { get; set; }
        
        /// <summary>
        /// Asunto del correo electrónico
        /// </summary>
        [Required(ErrorMessage = "El asunto del correo es obligatorio")]
        [StringLength(100, ErrorMessage = "El asunto no puede tener más de 100 caracteres")]
        public string Subject { get; set; }
        
        /// <summary>
        /// Contenido HTML de la plantilla
        /// </summary>
        [Required(ErrorMessage = "El contenido HTML es obligatorio")]
        public string HtmlContent { get; set; }
        
        /// <summary>
        /// Contenido de texto plano de la plantilla (alternativa sin HTML)
        /// </summary>
        [Required(ErrorMessage = "El contenido de texto plano es obligatorio")]
        public string TextContent { get; set; }
        
        /// <summary>
        /// Descripción de la plantilla
        /// </summary>
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string Description { get; set; }
        
        /// <summary>
        /// Indica si la plantilla está activa
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
