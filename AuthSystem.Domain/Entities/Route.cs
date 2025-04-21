using System;
using System.Collections.Generic;

namespace AuthSystem.Domain.Entities
{
    public class Route
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// Nombre de la ruta (debe ser único dentro del módulo)
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Descripción de la ruta
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// URL de la ruta
        /// </summary>
        public string Path { get; set; }
        
        /// <summary>
        /// Método HTTP (GET, POST, PUT, DELETE, etc.)
        /// </summary>
        public string HttpMethod { get; set; }
        
        /// <summary>
        /// Orden de visualización
        /// </summary>
        public int DisplayOrder { get; set; }
        
        /// <summary>
        /// Indica si la ruta requiere autenticación
        /// </summary>
        public bool RequiresAuth { get; set; }
        
        /// <summary>
        /// Indica si la ruta está habilitada
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// Indica si la ruta está activa
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// ID del módulo al que pertenece la ruta
        /// </summary>
        public Guid ModuleId { get; set; }
        
        /// <summary>
        /// Módulo al que pertenece la ruta
        /// </summary>
        public virtual Module Module { get; set; }
        
        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Usuario que creó la ruta
        /// </summary>
        public string CreatedBy { get; set; }
        
        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime LastModifiedAt { get; set; }
        
        /// <summary>
        /// Usuario que realizó la última actualización
        /// </summary>
        public string LastModifiedBy { get; set; }
        
        /// <summary>
        /// Colección de roles que tienen acceso a esta ruta
        /// </summary>
        public virtual ICollection<RoleRoute> RoleRoutes { get; set; }
    }
}
