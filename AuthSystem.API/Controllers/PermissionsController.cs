using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Models.Auth;
using AuthSystem.Domain.Models.Permissions;
using AuthSystem.Domain.Models.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de permisos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class PermissionsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionsController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="logger">Logger</param>
        public PermissionsController(
            IUnitOfWork unitOfWork,
            ILogger<PermissionsController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los permisos
        /// </summary>
        /// <returns>Lista de permisos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissions = await _unitOfWork.Permissions.GetAllAsync();
                var permissionDtos = permissions.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                }).ToList();

                return Ok(permissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener los permisos"
                });
            }
        }

        /// <summary>
        /// Obtiene un permiso por su ID
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <returns>Permiso</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PermissionDetailDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> GetPermissionById(Guid id)
        {
            try
            {
                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Permiso no encontrado"
                    });
                }

                var roles = await _unitOfWork.Roles.GetAllAsync();
                var rolePermissions = await _unitOfWork.RolePermissions.GetByPermissionAsync(id);
                var assignedRoleIds = rolePermissions.Select(rp => rp.RoleId).ToList();

                var permissionDto = new PermissionDetailDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description,
                    IsActive = permission.IsActive,
                    CreatedAt = permission.CreatedAt,
                    LastModifiedAt = permission.LastModifiedAt,
                    AssignedRoles = roles
                        .Where(r => assignedRoleIds.Contains(r.Id))
                        .Select(r => new RoleDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Description = r.Description,
                            IsActive = r.IsActive
                        }).ToList()
                };

                return Ok(permissionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID {PermissionId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener el permiso"
                });
            }
        }

        /// <summary>
        /// Crea un nuevo permiso
        /// </summary>
        /// <param name="request">Datos del permiso</param>
        /// <returns>Permiso creado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PermissionDto), 201)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de permiso inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Verificar si el nombre del permiso ya existe
                if (await _unitOfWork.Permissions.NameExistsAsync(request.Name))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre del permiso ya está en uso"
                    });
                }

                // Crear el permiso
                var permission = new Permission
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                    IsActive = true
                };

                await _unitOfWork.Permissions.AddAsync(permission);
                await _unitOfWork.SaveChangesAsync();

                var permissionDto = new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description
                };

                return CreatedAtAction(nameof(GetPermissionById), new { id = permission.Id }, permissionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo permiso");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al crear el permiso"
                });
            }
        }

        /// <summary>
        /// Actualiza un permiso existente
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <param name="request">Datos del permiso</param>
        /// <returns>Permiso actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> UpdatePermission(Guid id, [FromBody] UpdatePermissionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de permiso inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Permiso no encontrado"
                    });
                }

                // Verificar si el nombre del permiso ya existe
                if (!string.IsNullOrEmpty(request.Name) && 
                    request.Name != permission.Name && 
                    await _unitOfWork.Permissions.NameExistsAsync(request.Name, id))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El nombre del permiso ya está en uso"
                    });
                }

                // Actualizar los datos del permiso
                if (!string.IsNullOrEmpty(request.Name))
                {
                    permission.Name = request.Name;
                }

                if (!string.IsNullOrEmpty(request.Description))
                {
                    permission.Description = request.Description;
                }

                if (request.IsActive.HasValue)
                {
                    permission.IsActive = request.IsActive.Value;
                }

                permission.LastModifiedAt = DateTime.UtcNow;
                permission.LastModifiedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                await _unitOfWork.Permissions.UpdateAsync(permission);
                await _unitOfWork.SaveChangesAsync();

                var permissionDto = new PermissionDto
                {
                    Id = permission.Id,
                    Name = permission.Name,
                    Description = permission.Description
                };

                return Ok(permissionDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso con ID {PermissionId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al actualizar el permiso"
                });
            }
        }

        /// <summary>
        /// Elimina un permiso
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <returns>Respuesta de éxito</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> DeletePermission(Guid id)
        {
            try
            {
                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Permiso no encontrado"
                    });
                }

                // Verificar que no se está eliminando un permiso predeterminado
                if (permission.Name.StartsWith("users."))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "No se puede eliminar un permiso predeterminado del sistema"
                    });
                }

                // Eliminar el permiso (eliminación lógica)
                await _unitOfWork.Permissions.DeleteAsync(permission);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new SuccessResponse
                {
                    Message = "Permiso eliminado correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el permiso con ID {PermissionId}", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al eliminar el permiso"
                });
            }
        }

        /// <summary>
        /// Asigna un permiso a un rol
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <param name="request">Datos de asignación</param>
        /// <returns>Respuesta de éxito</returns>
        [HttpPost("{id}/assign-to-role")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 400)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> AssignPermissionToRole(Guid id, [FromBody] AssignPermissionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Datos de asignación inválidos",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Permiso no encontrado"
                    });
                }

                var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
                if (role == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Rol no encontrado"
                    });
                }

                // Verificar si el permiso ya está asignado al rol
                if (await _unitOfWork.RolePermissions.RoleHasPermissionAsync(request.RoleId, id))
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El permiso ya está asignado al rol"
                    });
                }

                // Asignar el permiso al rol
                var rolePermission = new RolePermission
                {
                    RoleId = request.RoleId,
                    PermissionId = id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System",
                    IsActive = true
                };

                await _unitOfWork.RolePermissions.AddAsync(rolePermission);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new SuccessResponse
                {
                    Message = "Permiso asignado correctamente al rol"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar el permiso con ID {PermissionId} al rol", id);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al asignar el permiso al rol"
                });
            }
        }

        /// <summary>
        /// Revoca un permiso de un rol
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Respuesta de éxito</returns>
        [HttpDelete("{id}/revoke-from-role/{roleId}")]
        [ProducesResponseType(typeof(SuccessResponse), 200)]
        [ProducesResponseType(typeof(ErrorResponse), 401)]
        [ProducesResponseType(typeof(ErrorResponse), 403)]
        [ProducesResponseType(typeof(ErrorResponse), 404)]
        [ProducesResponseType(typeof(ErrorResponse), 500)]
        public async Task<IActionResult> RevokePermissionFromRole(Guid id, Guid roleId)
        {
            try
            {
                var permission = await _unitOfWork.Permissions.GetByIdAsync(id);
                if (permission == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Permiso no encontrado"
                    });
                }

                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Rol no encontrado"
                    });
                }

                // Verificar si el permiso está asignado al rol
                var rolePermission = await _unitOfWork.RolePermissions.GetByRoleAndPermissionAsync(roleId, id);
                if (rolePermission == null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "El permiso no está asignado al rol"
                    });
                }

                // Revocar el permiso del rol
                await _unitOfWork.RolePermissions.DeleteAsync(rolePermission);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new SuccessResponse
                {
                    Message = "Permiso revocado correctamente del rol"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar el permiso con ID {PermissionId} del rol con ID {RoleId}", id, roleId);
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al revocar el permiso del rol"
                });
            }
        }
    }
}
