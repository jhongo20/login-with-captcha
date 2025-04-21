using System;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Permissions
{
    /// <summary>
    /// Modelo para la solicitud de asignación de permiso a un rol
    /// </summary>
    public class AssignPermissionRequest
    {
        /// <summary>
        /// ID del rol al que se asignará el permiso
        /// </summary>
        [Required(ErrorMessage = "El ID del rol es obligatorio")]
        public Guid RoleId { get; set; }
    }
}
