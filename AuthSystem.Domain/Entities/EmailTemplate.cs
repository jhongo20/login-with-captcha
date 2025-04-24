using System;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Representa una plantilla de correo electrónico en el sistema
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EmailTemplate()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        /// <summary>
        /// Identificador único de la plantilla
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nombre único de la plantilla (ej: "UserCreated", "PasswordReset")
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Asunto del correo electrónico
        /// </summary>
        public string Subject { get; set; } = string.Empty;
        
        /// <summary>
        /// Contenido HTML de la plantilla
        /// </summary>
        public string HtmlContent { get; set; } = string.Empty;
        
        /// <summary>
        /// Contenido de texto plano de la plantilla (alternativa sin HTML)
        /// </summary>
        public string TextContent { get; set; } = string.Empty;
        
        /// <summary>
        /// Indica si la plantilla está activa
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Descripción de la plantilla
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Usuario que creó la plantilla
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;
        
        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Usuario que realizó la última modificación
        /// </summary>
        public string? UpdatedBy { get; set; }
    }
}
