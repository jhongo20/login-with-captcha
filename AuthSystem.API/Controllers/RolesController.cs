using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Models.Auth;
using AuthSystem.Domain.Models.Roles;
using AuthSystem.Domain.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de roles
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RolesController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="logger">Logger</param>
        public RolesController(
            IUnitOfWork unitOfWork,
            ILogger<RolesController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los roles
        /// </summary>
        /// <returns>Lista de roles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _unitOfWork.Roles.GetAllAsync();
                var roleDtos = roles.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = r.IsActive,
                    CreatedAt = r.CreatedAt
                }).ToList();

                return Ok(roleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener los roles"
                });
            }
        }

        /// <summary>
        /// Obtiene un rol por su ID
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Rol</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDetailDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Rol no encontrado"
                    });
                }

                var permissions = await _unitOfWork.Permissions.GetByRoleAsync(id);

                var roleDto = new RoleDetailDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    CreatedAt = role.CreatedAt,
                    LastModifiedAt = role.LastModifiedAt,
                    Permissions = permissions.Select(p => new PermissionDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    }).ToList()
                };

                return Ok(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID {RoleId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener el rol"
                });
            }
        }

        /// <summary>
        /// Crea un nuevo rol
        /// </summary>
        /// <param name="request">Datos del rol</param>
        /// <returns>Rol creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RoleDto), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de rol inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Verificar si el nombre del rol ya existe
                if (await _unitOfWork.Roles.NameExistsAsync(request.Name))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre del rol ya está en uso"
                    });
                }

                // Crear el rol
                var role = new Role
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                    IsActive = true
                };

                await _unitOfWork.Roles.AddAsync(role);

                // Asignar permisos al rol
                if (request.PermissionIds != null && request.PermissionIds.Any())
                {
                    foreach (var permissionId in request.PermissionIds)
                    {
                        var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                        if (permission != null)
                        {
                            var rolePermission = new RolePermission
                            {
                                RoleId = role.Id,
                                PermissionId = permission.Id,
                                CreatedAt = DateTime.UtcNow,
                                CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                                IsActive = true
                            };

                            await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    CreatedAt = role.CreatedAt
                };

                return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo rol");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al crear el rol"
                });
            }
        }

        /// <summary>
        /// Actualiza un rol existente
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <param name="request">Datos del rol</param>
        /// <returns>Rol actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RoleDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de rol inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Rol no encontrado"
                    });
                }

                // Verificar si el nombre del rol ya existe
                if (!string.IsNullOrEmpty(request.Name) && 
                    request.Name != role.Name && 
                    await _unitOfWork.Roles.NameExistsAsync(request.Name, id))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre del rol ya está en uso"
                    });
                }

                // Actualizar los datos del rol
                if (!string.IsNullOrEmpty(request.Name))
                {
                    role.Name = request.Name;
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    role.Description = request.Description;
                }

                if (request.IsActive.HasValue)
                {
                    role.IsActive = request.IsActive.Value;
                }

                role.LastModifiedAt = DateTime.UtcNow;
                role.LastModifiedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                await _unitOfWork.Roles.UpdateAsync(role);

                // Actualizar permisos si se proporcionaron
                if (request.PermissionIds != null)
                {
                    // Obtener permisos actuales
                    var currentPermissions = await _unitOfWork.RolePermissions.GetByRoleAsync(id);
                    
                    // Eliminar permisos que ya no están en la lista
                    foreach (var rolePermission in currentPermissions)
                    {
                        if (!request.PermissionIds.Contains(rolePermission.PermissionId))
                        {
                            await _unitOfWork.RolePermissions.DeleteAsync(rolePermission);
                        }
                    }

                    // Agregar nuevos permisos
                    foreach (var permissionId in request.PermissionIds)
                    {
                        if (!currentPermissions.Any(rp => rp.PermissionId == permissionId))
                        {
                            var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                            if (permission != null)
                            {
                                var rolePermission = new RolePermission
                                {
                                    RoleId = role.Id,
                                    PermissionId = permissionId,
                                    CreatedAt = DateTime.UtcNow,
                                    CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                                    IsActive = true
                                };

                                await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                            }
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();

                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    CreatedAt = role.CreatedAt,
                    LastModifiedAt = role.LastModifiedAt
                };

                return Ok(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el rol con ID {RoleId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al actualizar el rol"
                });
            }
        }

        /// <summary>
        /// Elimina un rol
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Respuesta de éxito</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Rol no encontrado"
                    });
                }

                // Verificar que no se está eliminando un rol predeterminado
                if (role.Name == "Admin" || role.Name == "User")
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "No se puede eliminar un rol predeterminado del sistema"
                    });
                }

                // Eliminar el rol (eliminación lógica)
                await _unitOfWork.Roles.DeleteAsync(role);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new SuccessResponse
                {
                    Message = "Rol eliminado correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID {RoleId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al eliminar el rol"
                });
            }
        }

        /// <summary>
        /// Obtiene los usuarios asignados a un rol
        /// </summary>
        /// <param name="id">ID del rol</param>
        /// <returns>Lista de usuarios</returns>
        [HttpGet("{id}/users")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetRoleUsers(Guid id)
        {
            try
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(id);
                if (role == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Rol no encontrado"
                    });
                }

                var users = await _unitOfWork.Users.GetByRoleAsync(id);
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
                _logger.LogError(ex, "Error al obtener los usuarios del rol con ID {RoleId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener los usuarios del rol"
                });
            }
        }
    }
}
