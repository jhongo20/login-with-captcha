using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Solicitud de inicio de sesión con CAPTCHA
    /// </summary>
    public class LoginWithCaptchaRequest
    {
        /// <summary>
        /// Nombre de usuario o correo electrónico
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }

        /// <summary>
        /// ID del CAPTCHA
        /// </summary>
        [Required(ErrorMessage = "El ID del CAPTCHA es obligatorio")]
        public string CaptchaId { get; set; }

        /// <summary>
        /// Valor del CAPTCHA
        /// </summary>
        [Required(ErrorMessage = "El valor del CAPTCHA es obligatorio")]
        public string CaptchaValue { get; set; }

        /// <summary>
        /// Indica si el usuario es de LDAP
        /// </summary>
        public bool IsLdapUser { get; set; }
    }
}
