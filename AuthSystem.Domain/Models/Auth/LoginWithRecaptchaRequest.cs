using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para la solicitud de inicio de sesión con CAPTCHA
    /// </summary>
    public class LoginWithRecaptchaRequest
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }

        /// <summary>
        /// Indica si el usuario es de LDAP
        /// </summary>
        public bool IsLdapUser { get; set; } = false;

        /// <summary>
        /// Token de reCAPTCHA (para validación con Google reCAPTCHA)
        /// </summary>
        public string? RecaptchaToken { get; set; }
        
        /// <summary>
        /// ID del CAPTCHA (para validación con CAPTCHA interno)
        /// </summary>
        public string? CaptchaId { get; set; }
        
        /// <summary>
        /// Respuesta del CAPTCHA (para validación con CAPTCHA interno)
        /// </summary>
        public string? CaptchaResponse { get; set; }
    }
}
