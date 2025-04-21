using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Services;
using AuthSystem.Domain.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para la autenticación de usuarios
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly ICaptchaService _captchaService;
        private readonly IAccountLockoutService _accountLockoutService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="jwtService">Servicio JWT</param>
        /// <param name="captchaService">Servicio de CAPTCHA</param>
        /// <param name="accountLockoutService">Servicio de bloqueo de cuentas</param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Configuración</param>
        public AuthController(
            IUnitOfWork unitOfWork,
            IJwtService jwtService,
            ICaptchaService captchaService,
            IAccountLockoutService accountLockoutService,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _captchaService = captchaService ?? throw new ArgumentNullException(nameof(captchaService));
            _accountLockoutService = accountLockoutService ?? throw new ArgumentNullException(nameof(accountLockoutService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Inicia sesión con un usuario y contraseña
        /// </summary>
        /// <param name="request">Datos de inicio de sesión</param>
        /// <returns>Respuesta con el token JWT</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de inicio de sesión inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Buscar el usuario por nombre de usuario
                var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
                if (user == null)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Nombre de usuario o contraseña incorrectos"
                    });
                }

                // Verificar si la cuenta está bloqueada
                if (await _accountLockoutService.IsLockedOutAsync(user.Id))
                {
                    int remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(user.Id);
                    return StatusCode(403, new ErrorResponse
                    {
                        Message = $"La cuenta está bloqueada. Intente nuevamente en {remainingTime / 60} minutos.",
                        LockoutRemainingSeconds = remainingTime
                    });
                }

                // Verificar la contraseña
                bool isPasswordValid = false;
                
                if (request.IsLdapUser)
                {
                    // Aquí se implementaría la lógica para validar con LDAP
                    // Por ahora, simplemente rechazamos
                    isPasswordValid = false;
                }
                else
                {
                    // Verificar la contraseña con BCrypt
                    isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
                }

                if (!isPasswordValid)
                {
                    // Registrar intento fallido
                    bool isLocked = await _accountLockoutService.RecordFailedLoginAttemptAsync(user.Id);
                    
                    if (isLocked)
                    {
                        int remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(user.Id);
                        return StatusCode(403, new ErrorResponse
                        {
                            Message = $"La cuenta ha sido bloqueada debido a múltiples intentos fallidos. Intente nuevamente en {remainingTime / 60} minutos.",
                            LockoutRemainingSeconds = remainingTime
                        });
                    }
                    
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Nombre de usuario o contraseña incorrectos"
                    });
                }

                // Registrar inicio de sesión exitoso
                await _accountLockoutService.RecordSuccessfulLoginAsync(user.Id);

                // Obtener roles y permisos
                var roles = await _unitOfWork.Roles.GetByUserAsync(user.Id);
                var permissions = await _unitOfWork.Permissions.GetByUserAsync(user.Id);

                // Generar token JWT
                string token = await _jwtService.GenerateTokenAsync(
                    user.Id,
                    user.Username,
                    user.Email,
                    roles.Select(r => r.Name),
                    permissions.Select(p => p.Name));

                // Generar token de actualización
                string refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id);

                return Ok(new AuthResponse
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Token = token,
                    RefreshToken = refreshToken,
                    Roles = roles.Select(r => r.Name).ToArray(),
                    Permissions = permissions.Select(p => p.Name).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al procesar la solicitud de inicio de sesión"
                });
            }
        }

        /// <summary>
        /// Inicia sesión con un usuario, contraseña y CAPTCHA
        /// </summary>
        /// <param name="request">Datos de inicio de sesión con CAPTCHA</param>
        /// <returns>Respuesta con el token JWT</returns>
        [HttpPost("login-with-captcha")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> LoginWithCaptcha([FromBody] LoginWithRecaptchaRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de inicio de sesión inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Verificar si se proporcionó algún tipo de CAPTCHA
                if (string.IsNullOrEmpty(request.RecaptchaToken) && 
                    (string.IsNullOrEmpty(request.CaptchaId) || string.IsNullOrEmpty(request.CaptchaResponse)))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Se requiere proporcionar un token de reCAPTCHA o un CAPTCHA interno"
                    });
                }

                // Verificar el CAPTCHA
                bool isCaptchaValid = false;
                
                if (!string.IsNullOrEmpty(request.RecaptchaToken))
                {
                    // Verificar reCAPTCHA
                    isCaptchaValid = await _captchaService.ValidateReCaptchaAsync(request.RecaptchaToken);
                }
                else if (!string.IsNullOrEmpty(request.CaptchaId) && !string.IsNullOrEmpty(request.CaptchaResponse))
                {
                    // Verificar CAPTCHA interno
                    isCaptchaValid = _captchaService.ValidateCaptcha(request.CaptchaId, request.CaptchaResponse);
                }

                if (!isCaptchaValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "CAPTCHA inválido"
                    });
                }

                // Continuar con el proceso normal de inicio de sesión
                var loginRequest = new LoginRequest
                {
                    Username = request.Username,
                    Password = request.Password,
                    IsLdapUser = request.IsLdapUser
                };

                return await Login(loginRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión con CAPTCHA");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al procesar la solicitud de inicio de sesión"
                });
            }
        }

        /// <summary>
        /// Inicia sesión con un usuario, contraseña y Google reCAPTCHA
        /// </summary>
        /// <param name="request">Datos de inicio de sesión con Google reCAPTCHA</param>
        /// <returns>Respuesta con el token JWT</returns>
        [HttpPost("login-with-google-recaptcha")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> LoginWithGoogleRecaptcha([FromBody] LoginWithGoogleRecaptchaRequest request)
        {
            try
            {
                _logger.LogInformation("Intento de inicio de sesión con Google reCAPTCHA para el usuario: {Username}", request.Username);
                
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Modelo inválido para inicio de sesión con Google reCAPTCHA");
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de inicio de sesión inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Verificar el token de reCAPTCHA
                _logger.LogInformation("Verificando token de reCAPTCHA: {TokenLength} caracteres", 
                    request.RecaptchaToken?.Length ?? 0);
                
                bool isCaptchaValid = await _captchaService.ValidateReCaptchaAsync(request.RecaptchaToken);
                if (!isCaptchaValid)
                {
                    _logger.LogWarning("reCAPTCHA inválido para el usuario: {Username}", request.Username);
                    return BadRequest(new ErrorResponse
                    {
                        Message = "reCAPTCHA inválido o expirado"
                    });
                }

                _logger.LogInformation("reCAPTCHA válido, procediendo con el inicio de sesión");
                
                // Continuar con el proceso normal de inicio de sesión
                var loginRequest = new LoginRequest
                {
                    Username = request.Username,
                    Password = request.Password,
                    IsLdapUser = request.IsLdapUser
                };

                return await Login(loginRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar sesión con Google reCAPTCHA");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al procesar la solicitud de inicio de sesión: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Actualiza un token JWT utilizando un token de actualización
        /// </summary>
        /// <param name="request">Datos de actualización de token</param>
        /// <returns>Respuesta con el nuevo token JWT</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(AuthResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El token de actualización es obligatorio"
                    });
                }

                // Validar el token de actualización
                var (isValid, userId) = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken);
                if (!isValid || userId == Guid.Empty)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Token de actualización inválido o expirado"
                    });
                }

                // Obtener el usuario
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null || !user.IsActive)
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Usuario no encontrado o inactivo"
                    });
                }

                // Verificar si la cuenta está bloqueada
                if (await _accountLockoutService.IsLockedOutAsync(user.Id))
                {
                    int remainingTime = await _accountLockoutService.GetRemainingLockoutTimeAsync(user.Id);
                    return StatusCode(403, new ErrorResponse
                    {
                        Message = $"La cuenta está bloqueada. Intente nuevamente en {remainingTime / 60} minutos.",
                        LockoutRemainingSeconds = remainingTime
                    });
                }

                // Obtener roles y permisos
                var roles = await _unitOfWork.Roles.GetByUserAsync(user.Id);
                var permissions = await _unitOfWork.Permissions.GetByUserAsync(user.Id);

                // Generar nuevo token JWT
                string newToken = await _jwtService.GenerateTokenAsync(
                    user.Id,
                    user.Username,
                    user.Email,
                    roles.Select(r => r.Name),
                    permissions.Select(p => p.Name));

                // Generar nuevo token de actualización
                string newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id);

                // Actualizar la última actividad de la sesión
                var userSession = await _unitOfWork.UserSessions.GetByRefreshTokenAsync(request.RefreshToken);
                if (userSession != null)
                {
                    await _unitOfWork.UserSessions.UpdateLastActivityAsync(userSession.Id);
                }

                return Ok(new AuthResponse
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Token = newToken,
                    RefreshToken = newRefreshToken,
                    Roles = roles.Select(r => r.Name).ToArray(),
                    Permissions = permissions.Select(p => p.Name).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el token");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al procesar la solicitud de actualización de token"
                });
            }
        }

        /// <summary>
        /// Cierra la sesión de un usuario
        /// </summary>
        /// <param name="request">Datos de cierre de sesión</param>
        /// <returns>Respuesta de éxito</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.RefreshToken))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El token de actualización es obligatorio"
                    });
                }

                // Obtener el ID del usuario del token JWT
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
                {
                    return Unauthorized(new ErrorResponse
                    {
                        Message = "Token JWT inválido"
                    });
                }

                // Invalidar el token de actualización
                var userSession = await _unitOfWork.UserSessions.GetByRefreshTokenAsync(request.RefreshToken);
                if (userSession != null)
                {
                    // Verificar que el token pertenece al usuario autenticado
                    if (userSession.UserId != userId)
                    {
                        return Unauthorized(new ErrorResponse
                        {
                            Message = "El token de actualización no pertenece al usuario autenticado"
                        });
                    }

                    // Invalidar la sesión
                    await _unitOfWork.UserSessions.DeleteAsync(userSession);
                    await _unitOfWork.SaveChangesAsync();
                }

                return Ok(new SuccessResponse
                {
                    Message = "Sesión cerrada correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al procesar la solicitud de cierre de sesión"
                });
            }
        }

        /// <summary>
        /// Obtiene un CAPTCHA para el inicio de sesión
        /// </summary>
        /// <returns>Respuesta con la información del CAPTCHA</returns>
        [HttpGet("captcha")]
        [ProducesResponseType(typeof(CaptchaResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public IActionResult GetCaptcha()
        {
            try
            {
                // Obtener la clave pública de reCAPTCHA
                var recaptchaPublicKey = _configuration["Captcha:RecaptchaPublicKey"];
                
                // Si hay una clave pública configurada, devolver información para usar reCAPTCHA
                if (!string.IsNullOrEmpty(recaptchaPublicKey))
                {
                    return Ok(new CaptchaResponse
                    {
                        RecaptchaPublicKey = recaptchaPublicKey,
                        Message = "Por favor, complete el reCAPTCHA"
                    });
                }
                
                // Si no hay clave de reCAPTCHA, generar un CAPTCHA interno
                string captchaId = _captchaService.GenerateCaptcha();
                
                // Obtener la información del CAPTCHA
                var captchaInfo = _captchaService.GetCaptchaInfo(captchaId);
                
                if (captchaInfo == null)
                {
                    return StatusCode(500, new ErrorResponse
                    {
                        Message = "Error al generar CAPTCHA"
                    });
                }
                
                // Crear la respuesta
                var response = new CaptchaResponse
                {
                    CaptchaId = captchaId,
                    Question = captchaInfo.Question,
                    Options = captchaInfo.Options,
                    Message = "Por favor, responda la pregunta"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar CAPTCHA");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al generar CAPTCHA"
                });
            }
        }
    }
}
