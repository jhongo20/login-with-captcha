using System;
using System.Collections.Generic;
using AuthSystem.Domain.Models.Roles;

namespace AuthSystem.Domain.Models.Permissions
{
    /// <summary>
    /// DTO para información detallada de un permiso
    /// </summary>
    public class PermissionDetailDto
    {
        /// <summary>
        /// Identificador del permiso
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del permiso
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descripción del permiso
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indica si el permiso está activo
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
        /// Roles a los que está asignado este permiso
        /// </summary>
        public List<RoleDto> AssignedRoles { get; set; }
    }
}
