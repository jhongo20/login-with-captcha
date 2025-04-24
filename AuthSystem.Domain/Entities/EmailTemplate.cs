using System;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Representa una plantilla de correo electrónico en el sistema
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// Identificador único de la plantilla
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nombre único de la plantilla (ej: "UserCreated", "PasswordReset")
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Asunto del correo electrónico
        /// </summary>
        public string Subject { get; set; }
        
        /// <summary>
        /// Contenido HTML de la plantilla
        /// </summary>
        public string HtmlContent { get; set; }
        
        /// <summary>
        /// Contenido de texto plano de la plantilla (alternativa sin HTML)
        /// </summary>
        public string TextContent { get; set; }
        
        /// <summary>
        /// Indica si la plantilla está activa
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Descripción de la plantilla
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Usuario que creó la plantilla
        /// </summary>
        public string CreatedBy { get; set; }
        
        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime LastModifiedAt { get; set; }
        
        /// <summary>
        /// Usuario que realizó la última modificación
        /// </summary>
        public string LastModifiedBy { get; set; }
    }
}
