using AuthSystem.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using AuthSystem.Domain.Entities;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para gestionar las relaciones entre permisos y rutas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PermissionRoutesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PermissionRoutesController> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="logger">Logger</param>
        /// <param name="configuration">Configuración</param>
        public PermissionRoutesController(IUnitOfWork unitOfWork, ILogger<PermissionRoutesController> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene todos los permisos de una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Lista de permisos</returns>
        [HttpGet("by-route/{routeId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<object>>> GetPermissionsByRoute(Guid routeId)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                {
                    return NotFound($"No se encontró la ruta con ID {routeId}");
                }

                var permissions = await _unitOfWork.PermissionRoutes.GetPermissionsByRouteAsync(routeId);
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
                _logger.LogError(ex, "Error al obtener los permisos de la ruta {RouteId}", routeId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los permisos de la ruta");
            }
        }

        /// <summary>
        /// Obtiene todas las rutas que requieren un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Lista de rutas</returns>
        [HttpGet("by-permission/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<object>>> GetRoutesByPermission(Guid permissionId)
        {
            try
            {
                // Verificar que el permiso existe
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    return NotFound($"No se encontró el permiso con ID {permissionId}");
                }

                var routes = await _unitOfWork.PermissionRoutes.GetRoutesByPermissionAsync(permissionId);
                var routeDtos = routes.Select(r => new
                {
                    Id = r.Id,
                    Name = r.Name,
                    Path = r.Path,
                    HttpMethod = r.HttpMethod,
                    ModuleId = r.ModuleId,
                    ModuleName = r.Module?.Name
                }).ToList();

                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las rutas del permiso {PermissionId}", permissionId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las rutas del permiso");
            }
        }

        /// <summary>
        /// Asigna un permiso a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpPost("assign/{routeId}/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignPermissionToRoute(Guid routeId, Guid permissionId)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                {
                    _logger.LogWarning("No se encontró la ruta con ID {RouteId}", routeId);
                    return NotFound($"No se encontró la ruta con ID {routeId}");
                }

                // Verificar que el permiso existe
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    _logger.LogWarning("No se encontró el permiso con ID {PermissionId}", permissionId);
                    return NotFound($"No se encontró el permiso con ID {permissionId}");
                }

                // Obtener el nombre de usuario del token
                var userName = User.Identity.Name ?? "System";

                // Usar ADO.NET directamente para insertar la relación
                try
                {
                    // Verificar si ya existe la relación
                    string checkQuery = @"
                        SELECT COUNT(*) FROM PermissionRoutes 
                        WHERE RouteId = @RouteId AND PermissionId = @PermissionId";

                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        
                        // Verificar si la relación ya existe
                        using (var checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@RouteId", routeId);
                            checkCommand.Parameters.AddWithValue("@PermissionId", permissionId);
                            
                            int count = (int)await checkCommand.ExecuteScalarAsync();
                            
                            if (count > 0)
                            {
                                // Actualizar la relación existente
                                string updateQuery = @"
                                    UPDATE PermissionRoutes 
                                    SET IsActive = 1, 
                                        LastModifiedAt = @LastModifiedAt, 
                                        LastModifiedBy = @LastModifiedBy 
                                    WHERE RouteId = @RouteId AND PermissionId = @PermissionId";
                                
                                using (var updateCommand = new SqlCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@RouteId", routeId);
                                    updateCommand.Parameters.AddWithValue("@PermissionId", permissionId);
                                    updateCommand.Parameters.AddWithValue("@LastModifiedAt", DateTime.UtcNow);
                                    updateCommand.Parameters.AddWithValue("@LastModifiedBy", userName);
                                    
                                    await updateCommand.ExecuteNonQueryAsync();
                                    _logger.LogInformation("Relación PermissionRoute actualizada para RouteId: {RouteId}, PermissionId: {PermissionId}", routeId, permissionId);
                                }
                            }
                            else
                            {
                                // Crear una nueva relación
                                string insertQuery = @"
                                    INSERT INTO PermissionRoutes (
                                        Id, 
                                        PermissionId, 
                                        RouteId, 
                                        IsActive, 
                                        CreatedAt, 
                                        CreatedBy, 
                                        LastModifiedAt, 
                                        LastModifiedBy
                                    ) VALUES (
                                        @Id, 
                                        @PermissionId, 
                                        @RouteId, 
                                        1, 
                                        @CreatedAt, 
                                        @CreatedBy, 
                                        @LastModifiedAt, 
                                        @LastModifiedBy
                                    )";
                                
                                using (var insertCommand = new SqlCommand(insertQuery, connection))
                                {
                                    insertCommand.Parameters.AddWithValue("@Id", Guid.NewGuid());
                                    insertCommand.Parameters.AddWithValue("@RouteId", routeId);
                                    insertCommand.Parameters.AddWithValue("@PermissionId", permissionId);
                                    insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                                    insertCommand.Parameters.AddWithValue("@CreatedBy", userName);
                                    insertCommand.Parameters.AddWithValue("@LastModifiedAt", DateTime.UtcNow);
                                    insertCommand.Parameters.AddWithValue("@LastModifiedBy", userName);
                                    
                                    await insertCommand.ExecuteNonQueryAsync();
                                    _logger.LogInformation("Nueva relación PermissionRoute creada para RouteId: {RouteId}, PermissionId: {PermissionId}", routeId, permissionId);
                                }
                            }
                        }
                    }
                    
                    return Ok(new { message = $"Permiso asignado correctamente a la ruta" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear/actualizar la relación PermissionRoute para RouteId: {RouteId}, PermissionId: {PermissionId}", routeId, permissionId);
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error al asignar el permiso a la ruta: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar el permiso {PermissionId} a la ruta {RouteId}", permissionId, routeId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al asignar el permiso a la ruta: {ex.Message}");
            }
        }

        /// <summary>
        /// Revoca un permiso de una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpDelete("revoke/{routeId}/{permissionId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RevokePermissionFromRoute(Guid routeId, Guid permissionId)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                {
                    _logger.LogWarning("No se encontró la ruta con ID {RouteId}", routeId);
                    return NotFound($"No se encontró la ruta con ID {routeId}");
                }

                // Verificar que el permiso existe
                var permission = await _unitOfWork.Permissions.GetByIdAsync(permissionId);
                if (permission == null)
                {
                    _logger.LogWarning("No se encontró el permiso con ID {PermissionId}", permissionId);
                    return NotFound($"No se encontró el permiso con ID {permissionId}");
                }

                // Obtener el nombre de usuario del token
                var userName = User.Identity.Name ?? "System";

                // Usar ADO.NET directamente para actualizar la relación
                try
                {
                    // Verificar si existe la relación y está activa
                    string checkQuery = @"
                        SELECT COUNT(*) FROM PermissionRoutes 
                        WHERE RouteId = @RouteId AND PermissionId = @PermissionId AND IsActive = 1";

                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        
                        // Verificar si la relación existe y está activa
                        using (var checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@RouteId", routeId);
                            checkCommand.Parameters.AddWithValue("@PermissionId", permissionId);
                            
                            int count = (int)await checkCommand.ExecuteScalarAsync();
                            
                            if (count == 0)
                            {
                                return BadRequest($"El permiso no está asignado a la ruta");
                            }
                            
                            // Desactivar la relación
                            string updateQuery = @"
                                UPDATE PermissionRoutes 
                                SET IsActive = 0, 
                                    LastModifiedAt = @LastModifiedAt, 
                                    LastModifiedBy = @LastModifiedBy 
                                WHERE RouteId = @RouteId AND PermissionId = @PermissionId";
                            
                            using (var updateCommand = new SqlCommand(updateQuery, connection))
                            {
                                updateCommand.Parameters.AddWithValue("@RouteId", routeId);
                                updateCommand.Parameters.AddWithValue("@PermissionId", permissionId);
                                updateCommand.Parameters.AddWithValue("@LastModifiedAt", DateTime.UtcNow);
                                updateCommand.Parameters.AddWithValue("@LastModifiedBy", userName);
                                
                                await updateCommand.ExecuteNonQueryAsync();
                                _logger.LogInformation("Permiso revocado de la ruta. RouteId: {RouteId}, PermissionId: {PermissionId}", routeId, permissionId);
                            }
                        }
                    }
                    
                    return Ok(new { message = $"Permiso revocado correctamente de la ruta" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al revocar el permiso de la ruta. RouteId: {RouteId}, PermissionId: {PermissionId}", routeId, permissionId);
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error al revocar el permiso de la ruta: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar el permiso {PermissionId} de la ruta {RouteId}", permissionId, routeId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al revocar el permiso de la ruta: {ex.Message}");
            }
        }
    }
}
