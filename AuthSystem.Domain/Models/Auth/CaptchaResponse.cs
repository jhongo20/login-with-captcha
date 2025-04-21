using System.Collections.Generic;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para la respuesta de CAPTCHA
    /// </summary>
    public class CaptchaResponse
    {
        /// <summary>
        /// Indica si se requiere CAPTCHA
        /// </summary>
        public bool RequireCaptcha { get; set; }

        /// <summary>
        /// Identificador del CAPTCHA
        /// </summary>
        public string CaptchaId { get; set; }

        /// <summary>
        /// Pregunta del CAPTCHA
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Opciones de respuesta
        /// </summary>
        public List<string> Options { get; set; }

        /// <summary>
        /// Mensaje para el usuario
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// Clave pública de reCAPTCHA (si se usa Google reCAPTCHA)
        /// </summary>
        public string RecaptchaPublicKey { get; set; }
    }
}
