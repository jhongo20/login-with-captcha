using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para la solicitud de cierre de sesión
    /// </summary>
    public class LogoutRequest
    {
        /// <summary>
        /// Token de actualización
        /// </summary>
        [Required(ErrorMessage = "El token de actualización es obligatorio")]
        public string RefreshToken { get; set; }
    }
}
