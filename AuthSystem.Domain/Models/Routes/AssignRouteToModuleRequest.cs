using System;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Routes
{
    /// <summary>
    /// Modelo para asignar una ruta a un módulo
    /// </summary>
    public class AssignRouteToModuleRequest
    {
        /// <summary>
        /// ID de la ruta
        /// </summary>
        [Required(ErrorMessage = "El ID de la ruta es obligatorio")]
        public Guid RouteId { get; set; }

        /// <summary>
        /// ID del módulo
        /// </summary>
        [Required(ErrorMessage = "El ID del módulo es obligatorio")]
        public Guid ModuleId { get; set; }
    }
}
