using System;

namespace AuthSystem.Domain.Models.Roles
{
    /// <summary>
    /// DTO para información básica de un rol
    /// </summary>
    public class RoleDto
    {
        /// <summary>
        /// Identificador del rol
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del rol
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descripción del rol
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indica si el rol está activo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }
    }
}
