using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Roles
{
    /// <summary>
    /// Modelo para la solicitud de creación de rol
    /// </summary>
    public class CreateRoleRequest
    {
        /// <summary>
        /// Nombre del rol
        /// </summary>
        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del rol debe tener entre 3 y 50 caracteres")]
        public string Name { get; set; }

        /// <summary>
        /// Descripción del rol
        /// </summary>
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string Description { get; set; }

        /// <summary>
        /// IDs de los permisos a asignar al rol
        /// </summary>
        public List<Guid> PermissionIds { get; set; }
    }
}
