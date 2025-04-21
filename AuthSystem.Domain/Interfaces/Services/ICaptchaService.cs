using System.Threading.Tasks;
using AuthSystem.Domain.Models.Auth;
using System.Collections.Generic;

namespace AuthSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Interfaz para el servicio de CAPTCHA
    /// </summary>
    public interface ICaptchaService
    {
        /// <summary>
        /// Valida un token de reCAPTCHA
        /// </summary>
        /// <param name="token">Token de reCAPTCHA</param>
        /// <returns>True si el token es válido</returns>
        Task<bool> ValidateReCaptchaAsync(string token);

        /// <summary>
        /// Valida un CAPTCHA interno
        /// </summary>
        /// <param name="captchaId">ID del CAPTCHA</param>
        /// <param name="userAnswer">Respuesta del usuario</param>
        /// <returns>True si la respuesta es correcta</returns>
        bool ValidateCaptcha(string captchaId, string userAnswer);

        /// <summary>
        /// Genera un nuevo CAPTCHA
        /// </summary>
        /// <returns>ID del CAPTCHA generado</returns>
        string GenerateCaptcha();

        /// <summary>
        /// Obtiene la información de un CAPTCHA
        /// </summary>
        /// <param name="captchaId">ID del CAPTCHA</param>
        /// <returns>Información del CAPTCHA</returns>
        CaptchaInfo GetCaptchaInfo(string captchaId);
    }

    /// <summary>
    /// Información de un CAPTCHA
    /// </summary>
    public class CaptchaInfo
    {
        /// <summary>
        /// Pregunta del CAPTCHA
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Respuesta correcta
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Opciones de respuesta
        /// </summary>
        public List<string> Options { get; set; }
    }
}
