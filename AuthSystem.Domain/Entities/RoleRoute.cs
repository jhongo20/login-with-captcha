using System;

namespace AuthSystem.Domain.Entities
{
    public class RoleRoute
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// ID del rol
        /// </summary>
        public Guid RoleId { get; set; }
        
        /// <summary>
        /// Rol asociado
        /// </summary>
        public virtual Role Role { get; set; }
        
        /// <summary>
        /// ID de la ruta
        /// </summary>
        public Guid RouteId { get; set; }
        
        /// <summary>
        /// Ruta asociada
        /// </summary>
        public virtual Route Route { get; set; }
        
        /// <summary>
        /// Indica si la relación está activa
        /// </summary>
        public bool IsActive { get; set; }
        
        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Usuario que creó la relación
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
    }
}
