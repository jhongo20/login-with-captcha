using System;
using AuthSystem.Domain.Common;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad de relación entre usuarios y roles
    /// </summary>
    public class UserRole : BaseEntity
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// ID del rol
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// Usuario relacionado
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Rol relacionado
        /// </summary>
        public virtual Role Role { get; set; }
    }
}
