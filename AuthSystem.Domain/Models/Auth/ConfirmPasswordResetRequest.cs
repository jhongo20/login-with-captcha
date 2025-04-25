using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para confirmar el restablecimiento de contraseña
    /// </summary>
    public class ConfirmPasswordResetRequest
    {
        /// <summary>
        /// Token de restablecimiento de contraseña
        /// </summary>
        [Required(ErrorMessage = "El token de restablecimiento es obligatorio")]
        public string Token { get; set; }

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; }

        /// <summary>
        /// Nueva contraseña
        /// </summary>
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        public string NewPassword { get; set; }

        /// <summary>
        /// Confirmación de la nueva contraseña
        /// </summary>
        [Required(ErrorMessage = "La confirmación de contraseña es obligatoria")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }
    }
}
