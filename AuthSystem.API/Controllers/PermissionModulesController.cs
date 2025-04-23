using AuthSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para gestionar las relaciones entre permisos y módulos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionModulesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionModulesController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="logger">Logger</param>
        public PermissionModulesController(IUnitOfWork unitOfWork, ILogger<PermissionModulesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los permisos de un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>Lista de permisos</returns>
        [HttpGet("by-module/{moduleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<object>>> GetPermissionsByModule(Guid moduleId)
        {
            try
            {
                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return NotFound($"No se encontró el módulo con ID {moduleId}");
                }

                var permissions = await _unitOfWork.PermissionModules.GetPermissionsByModuleAsync(moduleId);
                var permissionDtos = permissions.Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                }).ToList();

                return Ok(permissionDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los permisos del módulo {ModuleId}", moduleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los permisos del módulo");
            }
        }

        /// <summary>
        /// Obtiene todos los módulos que requieren un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Lista de módulos</returns>
        [HttpGet("by-permission/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<object>>> GetModulesByPermission(Guid permissionId)
        {
            try
            {
                // Verificar que el permiso existe
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    return NotFound($"No se encontró el permiso con ID {permissionId}");
                }

                var modules = await _unitOfWork.PermissionModules.GetModulesByPermissionAsync(permissionId);
                var moduleDtos = modules.Select(m => new
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Route = m.Route,
                    Icon = m.Icon,
                    DisplayOrder = m.DisplayOrder
                }).ToList();

                return Ok(moduleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los módulos del permiso {PermissionId}", permissionId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los módulos del permiso");
            }
        }

        /// <summary>
        /// Asigna un permiso a un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpPost("assign/{moduleId}/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignPermissionToModule(Guid moduleId, Guid permissionId)
        {
            try
            {
                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return NotFound($"No se encontró el módulo con ID {moduleId}");
                }

                // Verificar que el permiso existe
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    return NotFound($"No se encontró el permiso con ID {permissionId}");
                }

                // Verificar si el permiso ya está asignado al módulo
                var isAssigned = await _unitOfWork.PermissionModules.ModuleRequiresPermissionAsync(moduleId, permissionId);
                if (isAssigned)
                {
                    return BadRequest($"El permiso ya está asignado al módulo");
                }

                // Obtener el nombre de usuario del token
                var userName = User.Identity.Name ?? "System";

                // Asignar el permiso al módulo
                await _unitOfWork.PermissionModules.AssignPermissionToModuleAsync(moduleId, permissionId, userName);

                return Ok(new { message = $"Permiso asignado correctamente al módulo" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al asignar el permiso {PermissionId} al módulo {ModuleId}", permissionId, moduleId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar el permiso {PermissionId} al módulo {ModuleId}", permissionId, moduleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al asignar el permiso al módulo");
            }
        }

        /// <summary>
        /// Revoca un permiso de un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpDelete("revoke/{moduleId}/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RevokePermissionFromModule(Guid moduleId, Guid permissionId)
        {
            try
            {
                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return NotFound($"No se encontró el módulo con ID {moduleId}");
                }

                // Verificar que el permiso existe
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    return NotFound($"No se encontró el permiso con ID {permissionId}");
                }

                // Verificar si el permiso está asignado al módulo
                var isAssigned = await _unitOfWork.PermissionModules.ModuleRequiresPermissionAsync(moduleId, permissionId);
                if (!isAssigned)
                {
                    return BadRequest($"El permiso no está asignado al módulo");
                }

                // Revocar el permiso del módulo
                await _unitOfWork.PermissionModules.RevokePermissionFromModuleAsync(moduleId, permissionId);

                return Ok(new { message = $"Permiso revocado correctamente del módulo" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al revocar el permiso {PermissionId} del módulo {ModuleId}", permissionId, moduleId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar el permiso {PermissionId} del módulo {ModuleId}", permissionId, moduleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al revocar el permiso del módulo");
            }
        }
    }
}
