using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Interfaces.Services;
using AuthSystem.Domain.Models.Auth;
using AuthSystem.Domain.Models.Users;
using AuthSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de usuarios
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountLockoutService _accountLockoutService;
        private readonly ILogger<UsersController> _logger;
        private readonly UserNotificationService _userNotificationService;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="accountLockoutService">Servicio de bloqueo de cuentas</param>
        /// <param name="logger">Logger</param>
        /// <param name="userNotificationService">Servicio de notificaciones de usuario</param>
        /// <param name="emailService">Servicio de correo electrónico</param>
        public UsersController(
            IUnitOfWork unitOfWork,
            IAccountLockoutService accountLockoutService,
            ILogger<UsersController> logger,
            UserNotificationService userNotificationService,
            IEmailService emailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _accountLockoutService = accountLockoutService ?? throw new ArgumentNullException(nameof(accountLockoutService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userNotificationService = userNotificationService ?? throw new ArgumentNullException(nameof(userNotificationService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Obtiene todos los usuarios
        /// </summary>
        /// <returns>Lista de usuarios</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllAsync();
                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    IsActive = u.IsActive,
                    UserType = u.UserType.ToString(),
                    CreatedAt = u.CreatedAt
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener los usuarios"
                });
            }
        }

        /// <summary>
        /// Obtiene todos los usuarios incluyendo los inactivos
        /// </summary>
        /// <returns>Lista de todos los usuarios, activos e inactivos</returns>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetAllUsersIncludingInactive()
        {
            try
            {
                var users = await _unitOfWork.Users.GetAllUsersIncludingInactiveAsync();
                var userDtos = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    IsActive = u.IsActive,
                    UserType = u.UserType.ToString(),
                    CreatedAt = u.CreatedAt
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios incluyendo inactivos");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener todos los usuarios"
                });
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                // Verificar si el usuario tiene permiso para ver este usuario
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && currentUserId != id.ToString())
                {
                    return Forbid();
                }

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Usuario no encontrado"
                    });
                }

                var roles = await _unitOfWork.Roles.GetByUserAsync(id);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    UserType = user.UserType.ToString(),
                    CreatedAt = user.CreatedAt,
                    Roles = roles.Select(r => r.Name).ToArray()
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {UserId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener el usuario"
                });
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID incluyendo inactivos
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Usuario</returns>
        [HttpGet("all/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetUserByIdIncludingInactive(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdIncludingInactiveAsync(id);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Usuario no encontrado"
                    });
                }

                var roles = await _unitOfWork.Roles.GetByUserAsync(id);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    UserType = user.UserType.ToString(),
                    CreatedAt = user.CreatedAt,
                    Roles = roles.Select(r => r.Name).ToArray()
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID {UserId} incluyendo inactivos", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener el usuario"
                });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        /// <param name="request">Datos del usuario</param>
        /// <returns>Usuario creado</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de usuario inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Validar que las contraseñas coincidan
                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Las contraseñas no coinciden"
                    });
                }

                // Validar formato de correo electrónico
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El formato del correo electrónico no es válido"
                    });
                }

                // Validar formato de nombre de usuario (solo letras, números y guiones bajos)
                if (!IsValidUsername(request.Username))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre de usuario solo puede contener letras, números y guiones bajos"
                    });
                }

                // Verificar si el nombre de usuario ya existe
                if (await _unitOfWork.Users.UsernameExistsAsync(request.Username))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre de usuario ya está en uso"
                    });
                }

                // Verificar si el correo electrónico ya existe
                if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El correo electrónico ya está en uso"
                    });
                }

                // Validar que se asigne al menos un rol
                if (request.Roles == null || !request.Roles.Any())
                {
                    // No hay problema, se asignará el rol por defecto
                    _logger.LogInformation("No se especificaron roles para el usuario. Se asignará el rol por defecto.");
                }
                else
                {
                    // Verificar que los roles especificados existan
                    foreach (var roleName in request.Roles)
                    {
                        var role = await _unitOfWork.Roles.GetByNameAsync(roleName);
                        if (role == null)
                        {
                            return BadRequest(new ErrorResponse
                            {
                                Message = $"El rol '{roleName}' no existe"
                            });
                        }
                    }
                }

                // Crear el usuario
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FullName = request.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    UserType = Domain.Common.Enums.UserType.Internal,
                    EmailConfirmed = false, // Cambiado a false para requerir activación
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                    IsActive = true
                };

                await _unitOfWork.Users.AddAsync(user);

                // Asignar roles al usuario
                if (request.Roles != null && request.Roles.Any())
                {
                    foreach (var roleName in request.Roles)
                    {
                        var role = await _unitOfWork.Roles.GetByNameAsync(roleName);
                        if (role != null)
                        {
                            var userRole = new UserRole
                            {
                                UserId = user.Id,
                                RoleId = role.Id,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                                IsActive = true
                            };

                            await _unitOfWork.UserRoles.AddAsync(userRole);
                        }
                    }
                }
                else
                {
                    // Asignar rol de usuario por defecto
                    var defaultRole = await _unitOfWork.Roles.GetByNameAsync("User");
                    if (defaultRole != null)
                    {
                        var userRole = new UserRole
                        {
                            UserId = user.Id,
                            RoleId = defaultRole.Id,
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                            IsActive = true
                        };

                        await _unitOfWork.UserRoles.AddAsync(userRole);
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Obtener los roles asignados
                var assignedRoles = await _unitOfWork.Roles.GetByUserAsync(user.Id);

                // Generar código de activación
                var activationCode = GenerateActivationCode();

                // Crear y guardar el código de activación en la base de datos
                var activation = new ActivationCode
                {
                    UserId = user.Id,
                    Code = activationCode,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    IsUsed = false
                };

                await _unitOfWork.ActivationCodes.AddAsync(activation);
                await _unitOfWork.SaveChangesAsync();

                // Enviar correo electrónico de bienvenida
                await _userNotificationService.SendWelcomeEmailAsync(user);

                // Enviar correo electrónico con código de activación
                try
                {
                    var templateData = new Dictionary<string, string>
                    {
                        { "FullName", user.FullName },
                        { "Username", user.Username },
                        { "Email", user.Email },
                        { "ActivationCode", activationCode },
                        { "ExpirationTime", "24 horas" }
                    };

                    await _emailService.SendEmailAsync(
                        "ActivationCode",
                        user.Email,
                        templateData);
                        
                    _logger.LogInformation($"Código de activación enviado a {user.Email}: {activationCode}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al enviar código de activación al usuario: {user.Email}");
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    UserType = user.UserType.ToString(),
                    CreatedAt = user.CreatedAt,
                    Roles = assignedRoles.Select(r => r.Name).ToArray()
                };

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo usuario");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al crear el usuario"
                });
            }
        }

        /// <summary>
        /// Activa la cuenta de un usuario utilizando un código de activación
        /// </summary>
        /// <param name="request">Datos para la activación de la cuenta</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("activate")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de activación inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Validar formato de correo electrónico
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El formato del correo electrónico no es válido"
                    });
                }

                // Validar que el código de activación no esté vacío
                if (string.IsNullOrWhiteSpace(request.ActivationCode))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El código de activación es obligatorio"
                    });
                }

                // Buscar el usuario por correo electrónico
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "No se encontró ningún usuario con el correo electrónico proporcionado"
                    });
                }

                // Verificar si la cuenta ya está activada
                if (user.EmailConfirmed)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "La cuenta ya está activada. Puede iniciar sesión directamente."
                    });
                }

                // Buscar el código de activación
                var activationCode = await _unitOfWork.ActivationCodes.GetByCodeAsync(request.ActivationCode);
                if (activationCode == null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El código de activación no es válido"
                    });
                }

                // Verificar que el código pertenezca al usuario
                if (activationCode.UserId != user.Id)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El código de activación no corresponde a este usuario"
                    });
                }

                // Verificar que el código no haya sido usado
                if (activationCode.IsUsed)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Este código de activación ya ha sido utilizado. Solicite un nuevo código."
                    });
                }

                // Verificar que el código no haya expirado
                if (activationCode.ExpiresAt < DateTime.UtcNow)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El código de activación ha expirado. Solicite un nuevo código."
                    });
                }

                // Marcar el código como utilizado
                await _unitOfWork.ActivationCodes.MarkAsUsedAsync(request.ActivationCode, "System");

                // Activar la cuenta del usuario
                user.EmailConfirmed = true;
                user.LastModifiedAt = DateTime.UtcNow;
                user.LastModifiedBy = "System";

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Enviar correo de confirmación de activación
                try
                {
                    var templateData = new Dictionary<string, string>
                    {
                        { "FullName", user.FullName },
                        { "Username", user.Username },
                        { "Email", user.Email },
                        { "ActivationDate", DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") }
                    };

                    await _emailService.SendEmailAsync(
                        "AccountActivated",
                        user.Email,
                        templateData);
                        
                    _logger.LogInformation($"Correo de confirmación de activación enviado a {user.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al enviar correo de confirmación de activación al usuario: {user.Email}");
                    // No devolvemos error al usuario porque la cuenta ya está activada
                }

                return Ok(new SuccessResponse
                {
                    Message = "Cuenta activada correctamente. Ya puede iniciar sesión."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al activar la cuenta del usuario");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al activar la cuenta. Por favor, inténtelo de nuevo más tarde."
                });
            }
        }

        /// <summary>
        /// Reenvía el código de activación a un usuario
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("resend-activation-code")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> ResendActivationCode([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El correo electrónico es obligatorio"
                    });
                }

                // Validar formato de correo electrónico
                if (!IsValidEmail(email))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El formato del correo electrónico no es válido"
                    });
                }

                // Buscar el usuario por correo electrónico
                var user = await _unitOfWork.Users.GetByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "No se encontró ningún usuario con el correo electrónico proporcionado"
                    });
                }

                // Verificar si la cuenta ya está activada
                if (user.EmailConfirmed)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "La cuenta ya está activada. Puede iniciar sesión directamente."
                    });
                }

                // Verificar si el usuario está activo
                if (!user.IsActive)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "La cuenta está desactivada. Contacte al administrador."
                    });
                }

                // Verificar límite de reenvíos por día (máximo 5 reenvíos por día)
                var today = DateTime.UtcNow.Date;
                var tomorrow = today.AddDays(1);
                var recentCodes = await _unitOfWork.ActivationCodes.FindAsync(
                    ac => ac.UserId == user.Id && ac.CreatedAt >= today && ac.CreatedAt < tomorrow);
                
                if (recentCodes.Count() >= 5)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Ha alcanzado el límite de reenvíos de códigos de activación por día. Inténtelo de nuevo mañana."
                    });
                }

                // Invalidar códigos anteriores
                var previousCodes = await _unitOfWork.ActivationCodes.FindAsync(ac => ac.UserId == user.Id && !ac.IsUsed && ac.ExpiresAt > DateTime.UtcNow);
                foreach (var code in previousCodes)
                {
                    code.IsUsed = true;
                    _unitOfWork.ActivationCodes.Update(code);
                }

                // Generar y guardar nuevo código de activación
                var activationCode = GenerateActivationCode();
                var expirationTime = DateTime.UtcNow.AddHours(24); // 24 horas de validez

                var activationCodeEntity = new ActivationCode
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Code = activationCode,
                    ExpiresAt = expirationTime,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                await _unitOfWork.ActivationCodes.AddAsync(activationCodeEntity);
                await _unitOfWork.SaveChangesAsync();

                // Enviar correo electrónico con código de activación
                try
                {
                    var templateData = new Dictionary<string, string>
                    {
                        { "FullName", user.FullName },
                        { "Username", user.Username },
                        { "Email", user.Email },
                        { "ActivationCode", activationCode },
                        { "ExpirationTime", "24 horas" }
                    };

                    await _emailService.SendEmailAsync(
                        "ActivationCode",
                        user.Email,
                        templateData);
                        
                    _logger.LogInformation($"Código de activación enviado a {user.Email}: {activationCode}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al enviar código de activación al usuario: {user.Email}");
                    return StatusCode(500, new ErrorResponse
                    {
                        Message = "Error al enviar el código de activación. Por favor, inténtelo de nuevo más tarde."
                    });
                }

                return Ok(new SuccessResponse
                {
                    Message = "Se ha enviado un nuevo código de activación a su correo electrónico. Por favor, revise su bandeja de entrada."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reenviar el código de activación");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al reenviar el código de activación. Por favor, inténtelo de nuevo más tarde."
                });
            }
        }

        /// <summary>
        /// Genera un código de activación aleatorio
        /// </summary>
        /// <returns>Código de activación</returns>
        private string GenerateActivationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="request">Datos del usuario</param>
        /// <returns>Usuario actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de usuario inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Verificar si el usuario tiene permiso para actualizar este usuario
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var isAdmin = User.IsInRole("Admin");

                if (!isAdmin && currentUserId != id.ToString())
                {
                    return Forbid();
                }

                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Usuario no encontrado"
                    });
                }

                // Verificar si el nombre de usuario ya existe
                if (!string.IsNullOrEmpty(request.Username) && 
                    request.Username != user.Username && 
                    await _unitOfWork.Users.UsernameExistsAsync(request.Username, id))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre de usuario ya está en uso"
                    });
                }

                // Verificar si el correo electrónico ya existe
                if (!string.IsNullOrEmpty(request.Email) && 
                    request.Email != user.Email && 
                    await _unitOfWork.Users.EmailExistsAsync(request.Email, id))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El correo electrónico ya está en uso"
                    });
                }

                // Actualizar los datos del usuario
                if (!string.IsNullOrEmpty(request.Username))
                {
                    user.Username = request.Username;
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    user.Email = request.Email;
                }

                if (!string.IsNullOrEmpty(request.FullName))
                {
                    user.FullName = request.FullName;
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                    user.SecurityStamp = Guid.NewGuid().ToString();
                }

                if (request.IsActive.HasValue)
                {
                    user.IsActive = request.IsActive.Value;
                }

                await _unitOfWork.Users.UpdateAsync(user);

                // Actualizar roles si el usuario es administrador
                if (isAdmin && request.Roles != null)
                {
                    // Obtener roles actuales
                    var currentRoles = await _unitOfWork.UserRoles.GetByUserAsync(id);
                    
                    // Eliminar roles que ya no están en la lista
                    foreach (var userRole in currentRoles)
                    {
                        if (!request.Roles.Contains(userRole.Role.Name))
                        {
                            await _unitOfWork.UserRoles.DeleteAsync(userRole);
                        }
                    }

                    // Agregar nuevos roles
                    foreach (var roleName in request.Roles)
                    {
                        var role = await _unitOfWork.Roles.GetByNameAsync(roleName);
                        if (role != null && !currentRoles.Any(ur => ur.RoleId == role.Id))
                        {
                            var userRole = new UserRole
                            {
                                UserId = user.Id,
                                RoleId = role.Id,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                                IsActive = true
                            };

                            await _unitOfWork.UserRoles.AddAsync(userRole);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                // Obtener los roles actualizados
                var updatedRoles = await _unitOfWork.Roles.GetByUserAsync(user.Id);

                // Enviar correo electrónico de actualización de cuenta
                await _userNotificationService.SendAccountUpdatedEmailAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    UserType = user.UserType.ToString(),
                    CreatedAt = user.CreatedAt,
                    Roles = updatedRoles.Select(r => r.Name).ToArray()
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID {UserId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al actualizar el usuario"
                });
            }
        }

        /// <summary>
        /// Actualiza un usuario existente incluyendo inactivos
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <param name="request">Datos del usuario</param>
        /// <returns>Usuario actualizado</returns>
        [HttpPut("all/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UpdateUserIncludingInactive(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de usuario inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var user = await _unitOfWork.Users.GetByIdIncludingInactiveAsync(id);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Usuario no encontrado"
                    });
                }

                // Verificar si el nombre de usuario ya existe
                if (!string.IsNullOrEmpty(request.Username) && 
                    request.Username != user.Username && 
                    await _unitOfWork.Users.UsernameExistsAsync(request.Username, id))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre de usuario ya está en uso"
                    });
                }

                // Verificar si el correo electrónico ya existe
                if (!string.IsNullOrEmpty(request.Email) && 
                    request.Email != user.Email && 
                    await _unitOfWork.Users.EmailExistsAsync(request.Email, id))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El correo electrónico ya está en uso"
                    });
                }

                // Actualizar los datos del usuario
                if (!string.IsNullOrEmpty(request.Username))
                {
                    user.Username = request.Username;
                }

                if (!string.IsNullOrEmpty(request.Email))
                {
                    user.Email = request.Email;
                }

                if (!string.IsNullOrEmpty(request.FullName))
                {
                    user.FullName = request.FullName;
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                }

                if (request.IsActive.HasValue)
                {
                    user.IsActive = request.IsActive.Value;
                }

                // Actualizar roles si se especifican
                if (request.Roles != null && request.Roles.Any())
                {
                    // Obtener roles existentes del usuario
                    var existingUserRoles = await _unitOfWork.UserRoles.GetByUserAsync(id);
                    
                    // Eliminar roles que ya no están en la lista
                    foreach (var userRole in existingUserRoles)
                    {
                        if (!request.Roles.Contains(userRole.Role.Name))
                        {
                            await _unitOfWork.UserRoles.DeleteAsync(userRole);
                        }
                    }

                    // Agregar nuevos roles
                    foreach (var roleName in request.Roles)
                    {
                        var role = await _unitOfWork.Roles.GetByNameAsync(roleName);
                        if (role != null && !existingUserRoles.Any(ur => ur.RoleId == role.Id))
                        {
                            var userRole = new UserRole
                            {
                                UserId = user.Id,
                                RoleId = role.Id,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                                IsActive = true
                            };

                            await _unitOfWork.UserRoles.AddAsync(userRole);
                        }
                    }
                }

                user.LastModifiedAt = DateTime.UtcNow;
                user.LastModifiedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                // Obtener roles actualizados
                var roles = await _unitOfWork.Roles.GetByUserAsync(id);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive,
                    UserType = user.UserType.ToString(),
                    CreatedAt = user.CreatedAt,
                    Roles = roles.Select(r => r.Name).ToArray()
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID {UserId} incluyendo inactivos", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al actualizar el usuario"
                });
            }
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Respuesta de éxito</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Usuario no encontrado"
                    });
                }

                // Verificar que no se está eliminando a sí mismo
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == id.ToString())
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "No puede eliminar su propia cuenta"
                    });
                }

                // Eliminar el usuario (eliminación lógica)
                await _unitOfWork.Users.DeleteAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new SuccessResponse
                {
                    Message = "Usuario eliminado correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID {UserId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al eliminar el usuario"
                });
            }
        }

        /// <summary>
        /// Desbloquea la cuenta de un usuario
        /// </summary>
        /// <param name="id">ID del usuario</param>
        /// <returns>Respuesta de éxito</returns>
        [HttpPost("{id}/unlock")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UnlockUser(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Usuario no encontrado"
                    });
                }

                // Desbloquear la cuenta
                await _accountLockoutService.UnlockAccountAsync(id);

                // Actualizar el usuario
                user.AccessFailedCount = 0;
                user.LockoutEnd = null;

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new SuccessResponse
                {
                    Message = "Cuenta desbloqueada correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desbloquear la cuenta del usuario con ID {UserId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al desbloquear la cuenta"
                });
            }
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            return emailRegex.IsMatch(email);
        }

        private bool IsValidUsername(string username)
        {
            var usernameRegex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9_-]+$");
            return usernameRegex.IsMatch(username);
        }
    }
}
