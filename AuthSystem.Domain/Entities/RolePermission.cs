using System;
using AuthSystem.Domain.Common;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad de relación entre roles y permisos
    /// </summary>
    public class RolePermission : BaseEntity
    {
        /// <summary>
        /// ID del rol
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// ID del permiso
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Rol relacionado
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// Permiso relacionado
        /// </summary>
        public virtual Permission Permission { get; set; }
    }
}
