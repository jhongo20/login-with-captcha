using System.Collections.Generic;
using AuthSystem.Domain.Common;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que representa un permiso en el sistema
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// Nombre del permiso
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descripción del permiso
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Roles que tienen este permiso
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Permission()
        {
            RolePermissions = new HashSet<RolePermission>();
        }
    }
}
