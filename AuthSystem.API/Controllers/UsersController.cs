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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="accountLockoutService">Servicio de bloqueo de cuentas</param>
        /// <param name="logger">Logger</param>
        /// <param name="userNotificationService">Servicio de notificaciones de usuario</param>
        public UsersController(
            IUnitOfWork unitOfWork,
            IAccountLockoutService accountLockoutService,
            ILogger<UsersController> logger,
            UserNotificationService userNotificationService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _accountLockoutService = accountLockoutService ?? throw new ArgumentNullException(nameof(accountLockoutService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userNotificationService = userNotificationService ?? throw new ArgumentNullException(nameof(userNotificationService));
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
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de usuario inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
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

                // Crear el usuario
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FullName = request.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    UserType = Domain.Common.Enums.UserType.Internal,
                    EmailConfirmed = true,
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

                // Enviar correo electrónico de bienvenida
                await _userNotificationService.SendWelcomeEmailAsync(user);

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

                user.LastModifiedAt = DateTime.UtcNow;
                user.LastModifiedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

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
                    LastModifiedAt = user.LastModifiedAt,
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
                user.LastModifiedAt = DateTime.UtcNow;
                user.LastModifiedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

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
    }
}
