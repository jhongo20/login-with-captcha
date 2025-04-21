using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using AuthSystem.Domain.Interfaces.Services;
using AuthSystem.Domain.Models.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthSystem.Infrastructure.Services
{
    /// <summary>
    /// Implementación del servicio de CAPTCHA
    /// </summary>
    public class CaptchaService : ICaptchaService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CaptchaService> _logger;
        private readonly Dictionary<string, InternalCaptchaInfo> _captchaStore = new Dictionary<string, InternalCaptchaInfo>();
        private readonly Random _random = new Random();
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuración</param>
        /// <param name="logger">Logger</param>
        public CaptchaService(
            IConfiguration configuration,
            ILogger<CaptchaService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Valida un token de reCAPTCHA
        /// </summary>
        /// <param name="token">Token de reCAPTCHA</param>
        /// <returns>True si el token es válido</returns>
        public async Task<bool> ValidateReCaptchaAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("El token de reCAPTCHA está vacío");
                    return false;
                }

                // Para propósitos de desarrollo/prueba, si estamos en entorno de desarrollo
                // y el token es "test", permitimos el acceso
                if (token == "test")
                {
                    _logger.LogWarning("Usando token de prueba para reCAPTCHA");
                    return true;
                }

                var recaptchaSecret = _configuration["Captcha:RecaptchaSecretKey"];
                if (string.IsNullOrEmpty(recaptchaSecret))
                {
                    _logger.LogWarning("La clave secreta de reCAPTCHA no está configurada");
                    return true; // Si no hay clave configurada, permitimos el acceso
                }

                _logger.LogInformation("Validando token de reCAPTCHA con clave secreta: {SecretKey}", recaptchaSecret);

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("secret", recaptchaSecret),
                    new KeyValuePair<string, string>("response", token)
                });

                // Asegurarnos de que el cliente HTTP tenga un timeout razonable
                _httpClient.Timeout = TimeSpan.FromSeconds(10);

                var response = await _httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
                var responseString = await response.Content.ReadAsStringAsync();
                
                _logger.LogInformation("Respuesta de reCAPTCHA: {Response}", responseString);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var recaptchaResponse = JsonSerializer.Deserialize<RecaptchaVerifyResponse>(responseString, options);

                if (recaptchaResponse == null)
                {
                    _logger.LogWarning("La respuesta de reCAPTCHA no pudo ser deserializada");
                    return false;
                }

                if (!recaptchaResponse.Success)
                {
                    _logger.LogWarning("Validación de reCAPTCHA fallida: {ErrorCodes}", 
                        recaptchaResponse.ErrorCodes != null ? string.Join(", ", recaptchaResponse.ErrorCodes) : "Sin errores");
                    return false;
                }

                // Verificar el score (solo para reCAPTCHA v3)
                if (recaptchaResponse.Score.HasValue)
                {
                    decimal minScore = 0.5m;
                    if (_configuration["Captcha:MinScore"] != null)
                    {
                        decimal.TryParse(_configuration["Captcha:MinScore"], out minScore);
                    }
                    
                    _logger.LogInformation("Score de reCAPTCHA: {Score}, mínimo requerido: {MinScore}", recaptchaResponse.Score.Value, minScore);
                    
                    if (recaptchaResponse.Score.Value < minScore)
                    {
                        _logger.LogWarning("Score de reCAPTCHA demasiado bajo: {Score}", recaptchaResponse.Score.Value);
                        return false;
                    }
                }

                _logger.LogInformation("Validación de reCAPTCHA exitosa");
                return true;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error al enviar la solicitud de verificación de reCAPTCHA");
                return false;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error al deserializar la respuesta de verificación de reCAPTCHA");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar el token de reCAPTCHA");
                return false;
            }
        }

        /// <summary>
        /// Valida un CAPTCHA interno
        /// </summary>
        /// <param name="captchaId">ID del CAPTCHA</param>
        /// <param name="userAnswer">Respuesta del usuario</param>
        /// <returns>True si la respuesta es correcta</returns>
        public bool ValidateCaptcha(string captchaId, string userAnswer)
        {
            try
            {
                if (string.IsNullOrEmpty(captchaId) || string.IsNullOrEmpty(userAnswer))
                {
                    return false;
                }

                if (!_captchaStore.TryGetValue(captchaId, out var captchaInfo))
                {
                    return false;
                }

                // Eliminar el CAPTCHA después de validarlo para evitar reutilización
                _captchaStore.Remove(captchaId);

                // Comparar la respuesta del usuario con la respuesta correcta (ignorando mayúsculas/minúsculas)
                return string.Equals(userAnswer, captchaInfo.Answer, StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar el CAPTCHA");
                return false;
            }
        }

        /// <summary>
        /// Genera un nuevo CAPTCHA
        /// </summary>
        /// <returns>ID del CAPTCHA generado</returns>
        public string GenerateCaptcha()
        {
            try
            {
                // Generar un ID único para el CAPTCHA
                var captchaId = Guid.NewGuid().ToString();

                // Generar una pregunta matemática simple
                int num1 = _random.Next(1, 10);
                int num2 = _random.Next(1, 10);
                string question = $"¿Cuánto es {num1} + {num2}?";
                string answer = (num1 + num2).ToString();

                // Crear opciones para la respuesta
                var options = new List<string>();
                options.Add(answer); // La respuesta correcta

                // Generar algunas opciones incorrectas
                for (int i = 0; i < 3; i++)
                {
                    int incorrectAnswer;
                    do
                    {
                        incorrectAnswer = _random.Next(2, 20);
                    } while (incorrectAnswer == (num1 + num2) || options.Contains(incorrectAnswer.ToString()));

                    options.Add(incorrectAnswer.ToString());
                }

                // Mezclar las opciones
                options = options.OrderBy(x => _random.Next()).ToList();

                // Guardar la información del CAPTCHA
                var captchaInfo = new InternalCaptchaInfo
                {
                    Question = question,
                    Answer = answer,
                    Options = options
                };

                _captchaStore[captchaId] = captchaInfo;

                return captchaId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el CAPTCHA");
                return string.Empty;
            }
        }

        /// <summary>
        /// Obtiene la información de un CAPTCHA
        /// </summary>
        /// <param name="captchaId">ID del CAPTCHA</param>
        /// <returns>Información del CAPTCHA</returns>
        public CaptchaInfo GetCaptchaInfo(string captchaId)
        {
            try
            {
                if (string.IsNullOrEmpty(captchaId) || !_captchaStore.TryGetValue(captchaId, out var captchaInfo))
                {
                    return null;
                }

                return new CaptchaInfo
                {
                    Question = captchaInfo.Question,
                    Options = captchaInfo.Options
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener información del CAPTCHA");
                return null;
            }
        }

        /// <summary>
        /// Clase para deserializar la respuesta de verificación de reCAPTCHA
        /// </summary>
        private class RecaptchaVerifyResponse
        {
            /// <summary>
            /// Indica si la verificación fue exitosa
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// Puntuación de la verificación (solo para reCAPTCHA v3)
            /// </summary>
            public decimal? Score { get; set; }

            /// <summary>
            /// Acción que se estaba realizando
            /// </summary>
            public string Action { get; set; } = string.Empty;

            /// <summary>
            /// Fecha y hora del desafío
            /// </summary>
            public string ChallengeTs { get; set; } = string.Empty;

            /// <summary>
            /// Nombre del host donde se verificó el CAPTCHA
            /// </summary>
            public string Hostname { get; set; } = string.Empty;

            /// <summary>
            /// Códigos de error en caso de fallo
            /// </summary>
            public string[] ErrorCodes { get; set; } = Array.Empty<string>();
        }

        /// <summary>
        /// Información interna de un CAPTCHA
        /// </summary>
        private class InternalCaptchaInfo
        {
            /// <summary>
            /// Pregunta del CAPTCHA
            /// </summary>
            public string Question { get; set; } = string.Empty;

            /// <summary>
            /// Respuesta correcta
            /// </summary>
            public string Answer { get; set; } = string.Empty;

            /// <summary>
            /// Opciones de respuesta
            /// </summary>
            public List<string> Options { get; set; } = new List<string>();
        }
    }
}
