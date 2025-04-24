using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Users
{
    /// <summary>
    /// Modelo para la solicitud de activación de cuenta
    /// </summary>
    public class ActivateAccountRequest
    {
        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido")]
        public string Email { get; set; }

        /// <summary>
        /// Código de activación
        /// </summary>
        [Required(ErrorMessage = "El código de activación es obligatorio")]
        [StringLength(10, MinimumLength = 6, ErrorMessage = "El código de activación debe tener entre 6 y 10 caracteres")]
        public string ActivationCode { get; set; }
    }
}
