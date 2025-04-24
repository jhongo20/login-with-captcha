using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using AuthSystem.Domain.Models.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthSystem.API.Controllers
{
    /// <summary>
    /// Controlador para gestionar las plantillas de correo electrónico
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class EmailTemplatesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<EmailTemplatesController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="logger">Logger</param>
        public EmailTemplatesController(IUnitOfWork unitOfWork, ILogger<EmailTemplatesController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las plantillas de correo electrónico
        /// </summary>
        /// <returns>Lista de plantillas de correo electrónico</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmailTemplateDto>>> GetAll()
        {
            try
            {
                var templates = await _unitOfWork.EmailTemplates.GetAllAsync();
                var templateDtos = templates.Select(t => new EmailTemplateDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Subject = t.Subject,
                    Description = t.Description,
                    IsActive = t.IsActive,
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy
                });

                return Ok(templateDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las plantillas de correo electrónico");
                return StatusCode(500, "Error interno del servidor al obtener las plantillas de correo electrónico");
            }
        }

        /// <summary>
        /// Obtiene una plantilla de correo electrónico por su ID
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        /// <returns>Plantilla de correo electrónico</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<EmailTemplate>> GetById(Guid id)
        {
            try
            {
                var template = await _unitOfWork.EmailTemplates.GetByIdAsync(id);
                if (template == null)
                {
                    return NotFound($"No se encontró la plantilla de correo electrónico con ID: {id}");
                }

                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la plantilla de correo electrónico con ID: {id}");
                return StatusCode(500, "Error interno del servidor al obtener la plantilla de correo electrónico");
            }
        }

        /// <summary>
        /// Crea una nueva plantilla de correo electrónico
        /// </summary>
        /// <param name="request">Datos de la plantilla</param>
        /// <returns>Plantilla creada</returns>
        [HttpPost]
        public async Task<ActionResult<EmailTemplate>> Create(CreateEmailTemplateRequest request)
        {
            try
            {
                // Verificar si ya existe una plantilla con el mismo nombre
                if (await _unitOfWork.EmailTemplates.ExistsByNameAsync(request.Name))
                {
                    return BadRequest($"Ya existe una plantilla con el nombre: {request.Name}");
                }

                // Obtener el nombre de usuario del token JWT
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                // Crear la plantilla
                var template = new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Subject = request.Subject,
                    HtmlContent = request.HtmlContent,
                    TextContent = request.TextContent,
                    Description = request.Description,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userName
                };

                // Guardar la plantilla
                await _unitOfWork.EmailTemplates.AddAsync(template);
                await _unitOfWork.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la plantilla de correo electrónico");
                return StatusCode(500, "Error interno del servidor al crear la plantilla de correo electrónico");
            }
        }

        /// <summary>
        /// Actualiza una plantilla de correo electrónico existente
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        /// <param name="request">Datos actualizados de la plantilla</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateEmailTemplateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("El ID de la ruta no coincide con el ID del objeto");
            }

            try
            {
                // Verificar si la plantilla existe
                var template = await _unitOfWork.EmailTemplates.GetByIdAsync(id);
                if (template == null)
                {
                    return NotFound($"No se encontró la plantilla de correo electrónico con ID: {id}");
                }

                // Obtener el nombre de usuario del token JWT
                var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";

                // Actualizar la plantilla
                // No actualizamos Name porque no está en el request
                template.Subject = request.Subject;
                template.HtmlContent = request.HtmlContent;
                template.TextContent = request.TextContent;
                template.Description = request.Description;
                template.IsActive = request.IsActive;

                // Guardar los cambios
                await _unitOfWork.EmailTemplates.UpdateAsync(template);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la plantilla de correo electrónico con ID: {id}");
                return StatusCode(500, "Error interno del servidor al actualizar la plantilla de correo electrónico");
            }
        }

        /// <summary>
        /// Elimina una plantilla de correo electrónico
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        /// <returns>Resultado de la operación</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                // Verificar si la plantilla existe
                var template = await _unitOfWork.EmailTemplates.GetByIdAsync(id);
                if (template == null)
                {
                    return NotFound($"No se encontró la plantilla de correo electrónico con ID: {id}");
                }

                // Eliminar la plantilla (marcar como inactiva)
                await _unitOfWork.EmailTemplates.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la plantilla de correo electrónico con ID: {id}");
                return StatusCode(500, "Error interno del servidor al eliminar la plantilla de correo electrónico");
            }
        }
    }
}
