using System.Collections.Generic;
using AuthSystem.Domain.Common;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que representa un rol en el sistema
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// Nombre del rol
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descripción del rol
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Usuarios asignados a este rol
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Permisos asignados a este rol
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            RolePermissions = new HashSet<RolePermission>();
        }
    }
}
