using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Models.Auth;
using AuthSystem.Domain.Models.Modules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de módulos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ModulesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ModulesController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="logger">Logger</param>
        public ModulesController(IUnitOfWork unitOfWork, ILogger<ModulesController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtiene todos los módulos
        /// </summary>
        /// <returns>Lista de módulos</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetAll()
        {
            try
            {
                var modules = await _unitOfWork.Modules.GetAllAsync();
                var rootModules = modules.Where(m => m.ParentId == null).ToList();
                var result = MapModulesToDtos(rootModules, modules.ToList());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los módulos");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener los módulos"
                });
            }
        }

        /// <summary>
        /// Obtiene un módulo por su ID
        /// </summary>
        /// <param name="id">ID del módulo</param>
        /// <returns>Módulo encontrado</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModuleDto>> GetById(Guid id)
        {
            try
            {
                var module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Módulo no encontrado"
                    });
                }

                var allModules = await _unitOfWork.Modules.GetAllAsync();
                var moduleDto = MapModuleToDto(module, allModules.ToList());
                return Ok(moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el módulo");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener el módulo"
                });
            }
        }

        /// <summary>
        /// Crea un nuevo módulo
        /// </summary>
        /// <param name="request">Datos del módulo</param>
        /// <returns>Módulo creado</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ModuleDto>> Create(CreateModuleRequest request)
        {
            try
            {
                // Validar si el módulo padre existe
                if (request.ParentId.HasValue)
                {
                    var parentModule = await _unitOfWork.Modules.GetByIdAsync(request.ParentId.Value);
                    if (parentModule == null)
                    {
                        return BadRequest(new ErrorResponse
                        {
                            Message = "El módulo padre no existe"
                        });
                    }
                }

                // Validar si ya existe un módulo con el mismo nombre
                var existingModule = (await _unitOfWork.Modules.GetAllAsync())
                    .FirstOrDefault(m => m.Name.ToLower() == request.Name.ToLower());
                if (existingModule != null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Ya existe un módulo con el mismo nombre"
                    });
                }

                // Crear el módulo
                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Route = request.Route,
                    Icon = request.Icon,
                    DisplayOrder = request.DisplayOrder,
                    ParentId = request.ParentId,
                    IsEnabled = request.IsEnabled,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = User.Identity.Name ?? "System",
                    IsActive = true
                };

                await _unitOfWork.Modules.AddAsync(module);
                await _unitOfWork.SaveChangesAsync();

                var allModules = await _unitOfWork.Modules.GetAllAsync();
                var moduleDto = MapModuleToDto(module, allModules.ToList());

                return CreatedAtAction(nameof(GetById), new { id = module.Id }, moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el módulo");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al crear el módulo"
                });
            }
        }

        /// <summary>
        /// Actualiza un módulo existente
        /// </summary>
        /// <param name="id">ID del módulo</param>
        /// <param name="request">Datos del módulo</param>
        /// <returns>Módulo actualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ModuleDto>> Update(Guid id, UpdateModuleRequest request)
        {
            try
            {
                // Validar si el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Módulo no encontrado"
                    });
                }

                // Validar si el módulo padre existe
                if (request.ParentId.HasValue)
                {
                    // No permitir que un módulo sea su propio padre
                    if (request.ParentId.Value == id)
                    {
                        return BadRequest(new ErrorResponse
                        {
                            Message = "Un módulo no puede ser su propio padre"
                        });
                    }

                    var parentModule = await _unitOfWork.Modules.GetByIdAsync(request.ParentId.Value);
                    if (parentModule == null)
                    {
                        return BadRequest(new ErrorResponse
                        {
                            Message = "El módulo padre no existe"
                        });
                    }

                    // Validar ciclos en la jerarquía
                    if (await HasCycle(id, request.ParentId.Value))
                    {
                        return BadRequest(new ErrorResponse
                        {
                            Message = "La jerarquía de módulos no puede tener ciclos"
                        });
                    }
                }

                // Validar si ya existe un módulo con el mismo nombre
                var existingModule = (await _unitOfWork.Modules.GetAllAsync())
                    .FirstOrDefault(m => m.Name.ToLower() == request.Name.ToLower() && m.Id != id);
                if (existingModule != null)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "Ya existe un módulo con el mismo nombre"
                    });
                }

                // Actualizar el módulo
                module.Name = request.Name;
                module.Description = request.Description;
                module.Route = request.Route;
                module.Icon = request.Icon;
                module.DisplayOrder = request.DisplayOrder;
                module.ParentId = request.ParentId;
                module.IsEnabled = request.IsEnabled;
                module.UpdatedAt = DateTime.UtcNow;
                module.UpdatedBy = User.Identity.Name ?? "System";

                await _unitOfWork.Modules.UpdateAsync(module);
                await _unitOfWork.SaveChangesAsync();

                var allModules = await _unitOfWork.Modules.GetAllAsync();
                var moduleDto = MapModuleToDto(module, allModules.ToList());

                return Ok(moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al actualizar el módulo"
                });
            }
        }

        /// <summary>
        /// Elimina un módulo
        /// </summary>
        /// <param name="id">ID del módulo</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                // Validar si el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Módulo no encontrado"
                    });
                }

                // Validar si tiene submódulos
                var hasChildren = await _unitOfWork.Modules.HasChildrenAsync(id);
                if (hasChildren)
                {
                    return BadRequest(new ErrorResponse
                    {
                        Message = "No se puede eliminar un módulo que tiene submódulos"
                    });
                }

                // Eliminar el módulo (borrado lógico)
                module.IsActive = false;
                module.UpdatedAt = DateTime.UtcNow;
                module.UpdatedBy = User.Identity.Name ?? "System";

                await _unitOfWork.Modules.UpdateAsync(module);
                await _unitOfWork.SaveChangesAsync();

                return Ok(new SuccessResponse
                {
                    Message = "Módulo eliminado correctamente"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el módulo");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al eliminar el módulo"
                });
            }
        }

        /// <summary>
        /// Obtiene todos los módulos habilitados
        /// </summary>
        /// <returns>Lista de módulos habilitados</returns>
        [HttpGet("enabled")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetEnabled()
        {
            try
            {
                var modules = await _unitOfWork.Modules.GetEnabledModulesAsync(true);
                var rootModules = modules.Where(m => m.ParentId == null).ToList();
                var result = MapModulesToDtos(rootModules, modules.ToList());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los módulos habilitados");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener los módulos habilitados"
                });
            }
        }

        /// <summary>
        /// Obtiene los submódulos de un módulo
        /// </summary>
        /// <param name="id">ID del módulo padre</param>
        /// <returns>Lista de submódulos</returns>
        [HttpGet("{id}/children")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetChildren(Guid id)
        {
            try
            {
                // Validar si el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(id);
                if (module == null)
                {
                    return NotFound(new ErrorResponse
                    {
                        Message = "Módulo no encontrado"
                    });
                }

                var children = await _unitOfWork.Modules.GetChildrenAsync(id);
                var allModules = await _unitOfWork.Modules.GetAllAsync();
                var result = MapModulesToDtos(children.ToList(), allModules.ToList());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los submódulos");
                return StatusCode(500, new ErrorResponse
                {
                    Message = "Error al obtener los submódulos"
                });
            }
        }

        /// <summary>
        /// Mapea un módulo a un DTO
        /// </summary>
        /// <param name="module">Módulo a mapear</param>
        /// <returns>DTO del módulo</returns>
        private ModuleDto MapToDto(Module module)
        {
            if (module == null)
            {
                return null;
            }

            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Route = module.Route,
                Icon = module.Icon,
                DisplayOrder = module.DisplayOrder,
                ParentId = module.ParentId,
                IsEnabled = module.IsEnabled,
                CreatedAt = module.CreatedAt,
                CreatedBy = module.CreatedBy,
                UpdatedAt = module.UpdatedAt,
                UpdatedBy = module.UpdatedBy
            };
        }

        /// <summary>
        /// Obtiene todos los módulos asignados a un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de módulos asignados al rol</returns>
        [HttpGet("byRole/{roleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModulesByRole(Guid roleId)
        {
            try
            {
                // Verificar que el rol existe
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound($"No se encontró el rol con ID {roleId}");
                }

                // Obtener los módulos asignados al rol
                var modules = await _unitOfWork.Modules.GetModulesByRoleAsync(roleId);
                var moduleDtos = modules.Select(MapToDto).ToList();

                return Ok(moduleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los módulos del rol con ID {RoleId}", roleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los módulos del rol");
            }
        }

        /// <summary>
        /// Asigna un módulo a un rol
        /// </summary>
        /// <param name="request">Datos de la asignación</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpPost("assign-to-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignModuleToRole([FromBody] AssignModuleToRoleRequest request)
        {
            try
            {
                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(request.ModuleId);
                if (module == null)
                {
                    return NotFound($"No se encontró el módulo con ID {request.ModuleId}");
                }

                // Verificar que el rol existe
                var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
                if (role == null)
                {
                    return NotFound($"No se encontró el rol con ID {request.RoleId}");
                }

                // Verificar si el rol ya tiene acceso al módulo
                if (await _unitOfWork.Modules.RoleHasModuleAccessAsync(request.RoleId, request.ModuleId))
                {
                    return BadRequest($"El rol ya tiene acceso al módulo");
                }

                // Obtener el nombre de usuario del token
                var userName = User.Identity.Name ?? "System";

                // Asignar el módulo al rol
                await _unitOfWork.Modules.AssignModuleToRoleAsync(request.ModuleId, request.RoleId, userName);

                return Ok(new { message = $"Módulo asignado correctamente al rol" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar el módulo {ModuleId} al rol {RoleId}", request.ModuleId, request.RoleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al asignar el módulo al rol");
            }
        }

        /// <summary>
        /// Revoca el acceso de un rol a un módulo
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpDelete("revoke-from-role/{roleId}/{moduleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RevokeModuleFromRole(Guid roleId, Guid moduleId)
        {
            try
            {
                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return NotFound($"No se encontró el módulo con ID {moduleId}");
                }

                // Verificar que el rol existe
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound($"No se encontró el rol con ID {roleId}");
                }

                // Verificar si el rol tiene acceso al módulo
                if (!await _unitOfWork.Modules.RoleHasModuleAccessAsync(roleId, moduleId))
                {
                    return BadRequest($"El rol no tiene acceso al módulo");
                }

                // Revocar el acceso del rol al módulo
                await _unitOfWork.Modules.RevokeModuleFromRoleAsync(moduleId, roleId);

                return Ok(new { message = $"Acceso al módulo revocado correctamente del rol" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar el acceso al módulo {ModuleId} del rol {RoleId}", moduleId, roleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al revocar el acceso al módulo del rol");
            }
        }

        #region Helper Methods

        /// <summary>
        /// Mapea una lista de módulos a DTOs
        /// </summary>
        /// <param name="modules">Lista de módulos</param>
        /// <param name="allModules">Todos los módulos</param>
        /// <returns>Lista de DTOs</returns>
        private List<ModuleDto> MapModulesToDtos(List<Module> modules, List<Module> allModules)
        {
            var result = new List<ModuleDto>();
            foreach (var module in modules)
            {
                result.Add(MapModuleToDto(module, allModules));
            }
            return result;
        }

        /// <summary>
        /// Mapea un módulo a DTO
        /// </summary>
        /// <param name="module">Módulo</param>
        /// <param name="allModules">Todos los módulos</param>
        /// <returns>DTO</returns>
        private ModuleDto MapModuleToDto(Module module, List<Module> allModules)
        {
            var dto = new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Route = module.Route,
                Icon = module.Icon,
                DisplayOrder = module.DisplayOrder,
                ParentId = module.ParentId,
                IsEnabled = module.IsEnabled,
                CreatedAt = module.CreatedAt,
                CreatedBy = module.CreatedBy,
                UpdatedAt = module.UpdatedAt,
                UpdatedBy = module.UpdatedBy ?? string.Empty
            };

            // Agregar submódulos
            var children = allModules.Where(m => m.ParentId == module.Id && m.IsActive).OrderBy(m => m.DisplayOrder).ToList();
            foreach (var child in children)
            {
                dto.Children.Add(MapModuleToDto(child, allModules));
            }

            return dto;
        }

        /// <summary>
        /// Verifica si hay un ciclo en la jerarquía de módulos
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="parentId">ID del módulo padre</param>
        /// <returns>True si hay un ciclo, False en caso contrario</returns>
        private async Task<bool> HasCycle(Guid moduleId, Guid parentId)
        {
            // Si el módulo es el mismo que el padre, hay un ciclo
            if (moduleId == parentId)
            {
                return true;
            }

            // Obtener el módulo padre
            var parent = await _unitOfWork.Modules.GetByIdAsync(parentId);
            if (parent == null)
            {
                return false;
            }

            // Si el padre no tiene padre, no hay ciclo
            if (!parent.ParentId.HasValue)
            {
                return false;
            }

            // Verificar si el padre del padre es el módulo original
            return await HasCycle(moduleId, parent.ParentId.Value);
        }

        #endregion
    }
}
