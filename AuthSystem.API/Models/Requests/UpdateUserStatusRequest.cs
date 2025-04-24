using System.ComponentModel.DataAnnotations;
using AuthSystem.Domain.Common.Enums;

namespace AuthSystem.API.Models.Requests
{
    /// <summary>
    /// Modelo para la solicitud de actualización del estado de un usuario
    /// </summary>
    public class UpdateUserStatusRequest
    {
        /// <summary>
        /// Nuevo estado del usuario
        /// </summary>
        [Required(ErrorMessage = "El estado del usuario es requerido")]
        public UserStatus Status { get; set; }
    }
}
