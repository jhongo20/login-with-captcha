using System;
using System.Collections.Generic;

namespace AuthSystem.Domain.Models.Roles
{
    /// <summary>
    /// DTO para información detallada de un rol
    /// </summary>
    public class RoleDetailDto
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

        /// <summary>
        /// Permisos asignados al rol
        /// </summary>
        public List<PermissionDto> Permissions { get; set; }
    }
}
