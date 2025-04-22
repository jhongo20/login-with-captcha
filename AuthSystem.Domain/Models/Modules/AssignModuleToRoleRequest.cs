using System;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Modules
{
    /// <summary>
    /// Modelo para asignar un módulo a un rol
    /// </summary>
    public class AssignModuleToRoleRequest
    {
        /// <summary>
        /// ID del módulo a asignar
        /// </summary>
        [Required]
        public Guid ModuleId { get; set; }
        
        /// <summary>
        /// ID del rol al que se asignará el módulo
        /// </summary>
        [Required]
        public Guid RoleId { get; set; }
    }
}
