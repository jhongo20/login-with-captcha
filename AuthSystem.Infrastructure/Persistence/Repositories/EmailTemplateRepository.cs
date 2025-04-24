using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthSystem.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementación del repositorio de plantillas de correo electrónico
    /// </summary>
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Contexto de la base de datos</param>
        public EmailTemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las plantillas de correo electrónico
        /// </summary>
        /// <returns>Lista de plantillas de correo electrónico</returns>
        public async Task<IEnumerable<EmailTemplate>> GetAllAsync()
        {
            return await _context.EmailTemplates
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una plantilla de correo electrónico por su ID
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        /// <returns>Plantilla de correo electrónico</returns>
        public async Task<EmailTemplate> GetByIdAsync(Guid id)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        /// <summary>
        /// Obtiene una plantilla de correo electrónico por su nombre
        /// </summary>
        /// <param name="name">Nombre de la plantilla</param>
        /// <returns>Plantilla de correo electrónico</returns>
        public async Task<EmailTemplate> GetByNameAsync(string name)
        {
            return await _context.EmailTemplates
                .FirstOrDefaultAsync(t => t.Name == name && t.IsActive);
        }

        /// <summary>
        /// Añade una nueva plantilla de correo electrónico
        /// </summary>
        /// <param name="template">Plantilla de correo electrónico</param>
        public async Task AddAsync(EmailTemplate template)
        {
            await _context.EmailTemplates.AddAsync(template);
        }

        /// <summary>
        /// Actualiza una plantilla de correo electrónico existente
        /// </summary>
        /// <param name="template">Plantilla de correo electrónico</param>
        public async Task UpdateAsync(EmailTemplate template)
        {
            _context.EmailTemplates.Update(template);
            await Task.CompletedTask;
        }

        /// <summary>
        /// Elimina una plantilla de correo electrónico
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        public async Task DeleteAsync(Guid id)
        {
            var template = await GetByIdAsync(id);
            if (template != null)
            {
                template.IsActive = false;
                _context.EmailTemplates.Update(template);
            }
        }

        /// <summary>
        /// Verifica si existe una plantilla con el nombre especificado
        /// </summary>
        /// <param name="name">Nombre de la plantilla</param>
        /// <returns>True si existe, False en caso contrario</returns>
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _context.EmailTemplates
                .AnyAsync(t => t.Name == name);
        }
    }
}
