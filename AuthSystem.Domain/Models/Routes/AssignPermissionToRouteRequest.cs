using System;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Routes
{
    /// <summary>
    /// Modelo para la asignación de un permiso a una ruta
    /// </summary>
    public class AssignPermissionToRouteRequest
    {
        /// <summary>
        /// ID de la ruta
        /// </summary>
        [Required(ErrorMessage = "El ID de la ruta es requerido")]
        public Guid RouteId { get; set; }

        /// <summary>
        /// ID del permiso
        /// </summary>
        [Required(ErrorMessage = "El ID del permiso es requerido")]
        public Guid PermissionId { get; set; }
    }
}
