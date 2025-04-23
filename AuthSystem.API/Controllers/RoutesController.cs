using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.SqlClient;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Models.Routes;
using AuthSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Route = AuthSystem.Domain.Entities.Route;

namespace AuthSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
    public class RoutesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoutesController> _logger;
        private readonly ApplicationDbContext _context; // Corregir el tipo de contexto
        private readonly IConfiguration _configuration;

        public RoutesController(IUnitOfWork unitOfWork, ILogger<RoutesController> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene todas las rutas
        /// </summary>
        /// <returns>Lista de rutas</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetAllRoutes()
        {
            try
            {
                _logger.LogInformation("Obteniendo todas las rutas");
                
                // Verificar si hay rutas en la base de datos directamente con el contexto
                var routeCount = await _context.Routes.CountAsync();
                _logger.LogInformation("Total de rutas en la base de datos: {Count}", routeCount);
                
                var activeRouteCount = await _context.Routes.Where(r => r.IsActive).CountAsync();
                _logger.LogInformation("Total de rutas activas en la base de datos: {Count}", activeRouteCount);
                
                // Obtener todas las rutas
                var routes = await _unitOfWork.Routes.GetAllAsync();
                _logger.LogInformation("Se encontraron {Count} rutas después de aplicar filtros", routes.Count());
                
                // Si no hay rutas, verificar si hay algún problema con la consulta
                if (!routes.Any())
                {
                    _logger.LogWarning("No se encontraron rutas activas. Verificando si hay rutas inactivas...");
                    var inactiveRoutes = await _context.Routes.Where(r => !r.IsActive).ToListAsync();
                    _logger.LogInformation("Rutas inactivas encontradas: {Count}", inactiveRoutes.Count);
                }
                
                var routeDtos = routes.Select(MapToDto).ToList();
                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las rutas: {Message}", ex.Message);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                }
                
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al obtener las rutas", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una ruta por su ID
        /// </summary>
        /// <param name="id">ID de la ruta</param>
        /// <returns>Ruta encontrada</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RouteDto>> GetRouteById(Guid id)
        {
            try
            {
                var route = await _unitOfWork.Routes.GetByIdAsync(id);
                if (route == null)
                {
                    return NotFound($"No se encontró la ruta con ID {id}");
                }

                return Ok(MapToDto(route));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la ruta con ID {RouteId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener la ruta");
            }
        }

        /// <summary>
        /// Obtiene todas las rutas habilitadas
        /// </summary>
        /// <returns>Lista de rutas habilitadas</returns>
        [HttpGet("enabled")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetEnabledRoutes()
        {
            try
            {
                var routes = await _unitOfWork.Routes.GetEnabledRoutesAsync();
                var routeDtos = routes.Select(MapToDto).ToList();
                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las rutas habilitadas");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las rutas habilitadas");
            }
        }

        /// <summary>
        /// Obtiene todas las rutas de un módulo específico
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>Lista de rutas del módulo</returns>
        [HttpGet("byModule/{moduleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesByModule(Guid moduleId)
        {
            try
            {
                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(moduleId);
                if (module == null)
                {
                    return NotFound($"No se encontró el módulo con ID {moduleId}");
                }

                var routes = await _unitOfWork.Routes.GetRoutesByModuleAsync(moduleId);
                var routeDtos = routes.Select(MapToDto).ToList();
                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las rutas del módulo con ID {ModuleId}", moduleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las rutas del módulo");
            }
        }

        /// <summary>
        /// Obtiene todas las rutas asignadas a un rol específico
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de rutas asignadas al rol</returns>
        [HttpGet("byRole/{roleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesByRole(Guid roleId)
        {
            try
            {
                // Verificar que el rol existe
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound($"No se encontró el rol con ID {roleId}");
                }

                var routes = await _unitOfWork.RoleRoutes.GetRoutesByRoleAsync(roleId);
                var routeDtos = routes.Select(MapToDto).ToList();
                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las rutas del rol con ID {RoleId}", roleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las rutas del rol");
            }
        }

        /// <summary>
        /// Crea una nueva ruta
        /// </summary>
        /// <param name="request">Datos de la ruta a crear</param>
        /// <returns>Ruta creada</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RouteDto>> CreateRoute([FromBody] CreateRouteRequest request)
        {
            try
            {
                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(request.ModuleId);
                if (module == null)
                {
                    _logger.LogWarning("No se encontró el módulo con ID {ModuleId}", request.ModuleId);
                    return BadRequest($"No se encontró el módulo con ID {request.ModuleId}");
                }

                // Verificar que no exista una ruta con el mismo nombre en el mismo módulo
                if (await _unitOfWork.Routes.ExistsWithNameInModuleAsync(request.Name, request.ModuleId))
                {
                    _logger.LogWarning("Ya existe una ruta con el nombre '{Name}' en el módulo seleccionado", request.Name);
                    return BadRequest($"Ya existe una ruta con el nombre '{request.Name}' en el módulo seleccionado");
                }

                // Verificar que no exista una ruta con el mismo path y método HTTP
                if (await _unitOfWork.Routes.ExistsWithPathAndMethodAsync(request.Path, request.HttpMethod))
                {
                    _logger.LogWarning("Ya existe una ruta con el path '{Path}' y método HTTP '{HttpMethod}'", request.Path, request.HttpMethod);
                    return BadRequest($"Ya existe una ruta con el path '{request.Path}' y método HTTP '{request.HttpMethod}'");
                }

                // Obtener el nombre de usuario del token
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                // Crear un nuevo ID para la ruta
                var routeId = Guid.NewGuid();
                
                // Crear la ruta directamente usando ADO.NET
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    var insertQuery = @"
                        INSERT INTO Routes (
                            Id, 
                            Name, 
                            Description, 
                            Path, 
                            HttpMethod, 
                            DisplayOrder, 
                            RequiresAuth, 
                            IsEnabled, 
                            IsActive, 
                            ModuleId, 
                            CreatedAt, 
                            CreatedBy, 
                            LastModifiedAt, 
                            LastModifiedBy
                        )
                        VALUES (
                            @Id, 
                            @Name, 
                            @Description, 
                            @Path, 
                            @HttpMethod, 
                            @DisplayOrder, 
                            @RequiresAuth, 
                            @IsEnabled, 
                            @IsActive, 
                            @ModuleId, 
                            @CreatedAt, 
                            @CreatedBy, 
                            @LastModifiedAt, 
                            @LastModifiedBy
                        );";
                    
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", routeId);
                        command.Parameters.AddWithValue("@Name", request.Name);
                        command.Parameters.AddWithValue("@Description", request.Description ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Path", request.Path);
                        command.Parameters.AddWithValue("@HttpMethod", request.HttpMethod);
                        command.Parameters.AddWithValue("@DisplayOrder", request.DisplayOrder);
                        command.Parameters.AddWithValue("@RequiresAuth", request.RequiresAuth);
                        command.Parameters.AddWithValue("@IsEnabled", request.IsEnabled);
                        command.Parameters.AddWithValue("@IsActive", true); // Forzar IsActive a true
                        command.Parameters.AddWithValue("@ModuleId", request.ModuleId);
                        command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@CreatedBy", userName);
                        command.Parameters.AddWithValue("@LastModifiedAt", DateTime.UtcNow);
                        command.Parameters.AddWithValue("@LastModifiedBy", userName);
                        
                        _logger.LogInformation("Ejecutando consulta SQL para crear ruta: {Name}, Path: {Path}, HttpMethod: {HttpMethod}, ModuleId: {ModuleId}, IsActive: {IsActive}", 
                            request.Name, request.Path, request.HttpMethod, request.ModuleId, true);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }
                
                // Obtener la ruta recién creada
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                {
                    _logger.LogError("No se pudo obtener la ruta recién creada con ID {RouteId}", routeId);
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear la ruta");
                }
                
                // Cargar el módulo manualmente
                route.Module = module;
                
                var routeDto = MapToDto(route);
                return CreatedAtAction(nameof(GetRouteById), new { id = routeDto.Id }, routeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la ruta: {Message}", ex.Message);
                
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);
                }
                
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al crear la ruta: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza una ruta existente
        /// </summary>
        /// <param name="id">ID de la ruta a actualizar</param>
        /// <param name="request">Datos actualizados de la ruta</param>
        /// <returns>Ruta actualizada</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<RouteDto>> UpdateRoute(Guid id, [FromBody] UpdateRouteRequest request)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(id);
                if (route == null)
                {
                    return NotFound($"No se encontró la ruta con ID {id}");
                }

                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(request.ModuleId);
                if (module == null)
                {
                    return BadRequest($"No se encontró el módulo con ID {request.ModuleId}");
                }

                // Verificar que no exista otra ruta con el mismo nombre en el mismo módulo
                if (await _unitOfWork.Routes.ExistsWithNameInModuleAsync(request.Name, request.ModuleId, id))
                {
                    return BadRequest($"Ya existe otra ruta con el nombre '{request.Name}' en el módulo seleccionado");
                }

                // Verificar que no exista otra ruta con el mismo path y método HTTP
                if (await _unitOfWork.Routes.ExistsWithPathAndMethodAsync(request.Path, request.HttpMethod, id))
                {
                    return BadRequest($"Ya existe otra ruta con el path '{request.Path}' y método HTTP '{request.HttpMethod}'");
                }

                // Obtener el nombre de usuario del token
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                // Actualizar la ruta
                route.Name = request.Name;
                route.Description = request.Description;
                route.Path = request.Path;
                route.HttpMethod = request.HttpMethod;
                route.DisplayOrder = request.DisplayOrder;
                route.RequiresAuth = request.RequiresAuth;
                route.IsEnabled = request.IsEnabled;
                route.ModuleId = request.ModuleId;
                route.LastModifiedAt = DateTime.UtcNow;
                route.LastModifiedBy = userName;

                _unitOfWork.Routes.Update(route);
                await _unitOfWork.SaveChangesAsync();

                // Obtener la ruta actualizada con su módulo para el DTO
                route = await _unitOfWork.Routes.GetByIdAsync(id);

                return Ok(MapToDto(route));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la ruta con ID {RouteId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al actualizar la ruta");
            }
        }

        /// <summary>
        /// Elimina una ruta
        /// </summary>
        /// <param name="id">ID de la ruta a eliminar</param>
        /// <returns>No Content</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DeleteRoute(Guid id)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(id);
                if (route == null)
                {
                    return NotFound($"No se encontró la ruta con ID {id}");
                }

                // Desactivar la ruta en lugar de eliminarla físicamente
                route.IsActive = false;
                route.LastModifiedAt = DateTime.UtcNow;
                route.LastModifiedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                _unitOfWork.Routes.Update(route);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la ruta con ID {RouteId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar la ruta");
            }
        }

        /// <summary>
        /// Asigna una ruta a un rol
        /// </summary>
        /// <param name="request">Datos de la asignación</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpPost("assign")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignRouteToRole([FromBody] AssignRouteToRoleRequest request)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(request.RouteId);
                if (route == null)
                {
                    _logger.LogWarning("No se encontró la ruta con ID {RouteId}", request.RouteId);
                    return NotFound($"No se encontró la ruta con ID {request.RouteId}");
                }

                // Verificar que el rol existe
                var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId);
                if (role == null)
                {
                    _logger.LogWarning("No se encontró el rol con ID {RoleId}", request.RoleId);
                    return NotFound($"No se encontró el rol con ID {request.RoleId}");
                }

                // Obtener el nombre de usuario del token
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                // Usar ADO.NET directamente para insertar la relación
                try
                {
                    // Verificar si ya existe la relación
                    string checkQuery = @"
                        SELECT COUNT(*) FROM RoleRoutes 
                        WHERE RouteId = @RouteId AND RoleId = @RoleId";

                    string connectionString = _configuration.GetConnectionString("DefaultConnection");
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        
                        // Verificar si la relación ya existe
                        using (var checkCommand = new SqlCommand(checkQuery, connection))
                        {
                            checkCommand.Parameters.AddWithValue("@RouteId", request.RouteId);
                            checkCommand.Parameters.AddWithValue("@RoleId", request.RoleId);
                            
                            int count = (int)await checkCommand.ExecuteScalarAsync();
                            
                            if (count > 0)
                            {
                                // Actualizar la relación existente
                                string updateQuery = @"
                                    UPDATE RoleRoutes 
                                    SET IsActive = 1, 
                                        LastModifiedAt = @LastModifiedAt, 
                                        LastModifiedBy = @LastModifiedBy 
                                    WHERE RouteId = @RouteId AND RoleId = @RoleId";
                                
                                using (var updateCommand = new SqlCommand(updateQuery, connection))
                                {
                                    updateCommand.Parameters.AddWithValue("@RouteId", request.RouteId);
                                    updateCommand.Parameters.AddWithValue("@RoleId", request.RoleId);
                                    updateCommand.Parameters.AddWithValue("@LastModifiedAt", DateTime.UtcNow);
                                    updateCommand.Parameters.AddWithValue("@LastModifiedBy", userName);
                                    
                                    await updateCommand.ExecuteNonQueryAsync();
                                    _logger.LogInformation("Relación RoleRoute actualizada para RouteId: {RouteId}, RoleId: {RoleId}", request.RouteId, request.RoleId);
                                }
                            }
                            else
                            {
                                // Crear una nueva relación
                                string insertQuery = @"
                                    INSERT INTO RoleRoutes (
                                        Id, 
                                        RoleId, 
                                        RouteId, 
                                        IsActive, 
                                        CreatedAt, 
                                        CreatedBy, 
                                        LastModifiedAt, 
                                        LastModifiedBy
                                    ) VALUES (
                                        @Id, 
                                        @RoleId, 
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
                                    insertCommand.Parameters.AddWithValue("@RouteId", request.RouteId);
                                    insertCommand.Parameters.AddWithValue("@RoleId", request.RoleId);
                                    insertCommand.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                                    insertCommand.Parameters.AddWithValue("@CreatedBy", userName);
                                    insertCommand.Parameters.AddWithValue("@LastModifiedAt", DateTime.UtcNow);
                                    insertCommand.Parameters.AddWithValue("@LastModifiedBy", userName);
                                    
                                    await insertCommand.ExecuteNonQueryAsync();
                                    _logger.LogInformation("Nueva relación RoleRoute creada para RouteId: {RouteId}, RoleId: {RoleId}", request.RouteId, request.RoleId);
                                }
                            }
                        }
                    }
                    
                    return Ok(new { message = $"Ruta asignada correctamente al rol" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear/actualizar la relación RoleRoute para RouteId: {RouteId}, RoleId: {RoleId}", request.RouteId, request.RoleId);
                    return StatusCode(StatusCodes.Status500InternalServerError, $"Error al asignar la ruta al rol: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar la ruta {RouteId} al rol {RoleId}", request.RouteId, request.RoleId);
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al asignar la ruta al rol: {ex.Message}");
            }
        }

        /// <summary>
        /// Revoca el acceso de un rol a una ruta
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpDelete("revoke/{roleId}/{routeId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RevokeRouteFromRole(Guid roleId, Guid routeId)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                {
                    return NotFound($"No se encontró la ruta con ID {routeId}");
                }

                // Verificar que el rol existe
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId);
                if (role == null)
                {
                    return NotFound($"No se encontró el rol con ID {roleId}");
                }

                // Verificar que el rol tiene acceso a la ruta
                if (!await _unitOfWork.RoleRoutes.RoleHasRouteAsync(routeId, roleId))
                {
                    return BadRequest($"El rol no tiene acceso a esta ruta");
                }

                // Revocar el acceso del rol a la ruta
                await _unitOfWork.RoleRoutes.RevokeRouteFromRoleAsync(routeId, roleId);

                return Ok(new { message = $"Acceso a la ruta revocado correctamente del rol" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al revocar el acceso a la ruta {RouteId} del rol {RoleId}", routeId, roleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al revocar el acceso a la ruta del rol");
            }
        }

        /// <summary>
        /// Obtiene todas las rutas de un módulo específico a las que tiene acceso un rol
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de rutas del módulo a las que tiene acceso el rol</returns>
        [HttpGet("byModuleAndRole/{moduleId}/{roleId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesByModuleAndRole(Guid moduleId, Guid roleId)
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

                // Obtener las rutas del módulo asignadas al rol
                var routes = await _unitOfWork.RoleRoutes.GetRoutesByModuleAndRoleAsync(moduleId, roleId);
                var routeDtos = routes.Select(MapToDto).ToList();

                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las rutas del módulo {ModuleId} para el rol {RoleId}", moduleId, roleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las rutas del módulo para el rol");
            }
        }

        /// <summary>
        /// Asigna una ruta a un módulo
        /// </summary>
        /// <param name="request">Datos de la asignación</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpPost("assign-to-module")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> AssignRouteToModule([FromBody] AssignRouteToModuleRequest request)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(request.RouteId);
                if (route == null)
                {
                    return NotFound($"No se encontró la ruta con ID {request.RouteId}");
                }

                // Verificar que el módulo existe
                var module = await _unitOfWork.Modules.GetByIdAsync(request.ModuleId);
                if (module == null)
                {
                    return NotFound($"No se encontró el módulo con ID {request.ModuleId}");
                }

                // Verificar si la ruta ya está asignada al módulo
                if (route.ModuleId == request.ModuleId)
                {
                    return BadRequest($"La ruta ya está asignada al módulo");
                }

                // Obtener el nombre de usuario del token
                var userName = User.Identity.Name ?? "System";

                // Asignar la ruta al módulo
                await _unitOfWork.Routes.AssignRouteToModuleAsync(request.RouteId, request.ModuleId, userName);

                return Ok(new { message = $"Ruta asignada correctamente al módulo" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar la ruta {RouteId} al módulo {ModuleId}", request.RouteId, request.ModuleId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al asignar la ruta al módulo");
            }
        }

        /// <summary>
        /// Revoca una ruta de un módulo (la desvincula)
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Mensaje de confirmación</returns>
        [HttpDelete("revoke-from-module/{routeId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> RevokeRouteFromModule(Guid routeId)
        {
            try
            {
                // Verificar que la ruta existe
                var route = await _unitOfWork.Routes.GetByIdAsync(routeId);
                if (route == null)
                {
                    return NotFound($"No se encontró la ruta con ID {routeId}");
                }

                // Obtener el nombre de usuario del token
                var userName = User.Identity.Name ?? "System";

                // Revocar la ruta del módulo
                await _unitOfWork.Routes.RevokeRouteFromModuleAsync(routeId, userName);

                return Ok(new { message = $"Ruta desvinculada correctamente del módulo" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error al desvincular la ruta {RouteId} del módulo", routeId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desvincular la ruta {RouteId} del módulo", routeId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al desvincular la ruta del módulo");
            }
        }

        /// <summary>
        /// Obtiene todas las rutas que no están vinculadas a ningún módulo
        /// </summary>
        /// <returns>Lista de rutas sin módulo</returns>
        [HttpGet("without-module")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<RouteDto>>> GetRoutesWithoutModule()
        {
            try
            {
                var routes = await _unitOfWork.Routes.GetRoutesWithoutModuleAsync();
                var routeDtos = routes.Select(MapToDto).ToList();
                return Ok(routeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las rutas sin módulo");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener las rutas sin módulo");
            }
        }

        /// <summary>
        /// Mapea una entidad Route a un DTO RouteDto
        /// </summary>
        private RouteDto MapToDto(AuthSystem.Domain.Entities.Route route)
        {
            return new RouteDto
            {
                Id = route.Id,
                Name = route.Name,
                Description = route.Description,
                Path = route.Path,
                HttpMethod = route.HttpMethod,
                DisplayOrder = route.DisplayOrder,
                RequiresAuth = route.RequiresAuth,
                IsEnabled = route.IsEnabled,
                IsActive = route.IsActive,
                ModuleId = route.ModuleId,
                ModuleName = route.Module?.Name,
                CreatedAt = route.CreatedAt,
                CreatedBy = route.CreatedBy,
                LastModifiedAt = route.LastModifiedAt,
                LastModifiedBy = route.LastModifiedBy
            };
        }
    }
}
