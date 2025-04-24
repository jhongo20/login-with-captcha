using AuthSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de plantillas de correo electrónico
    /// </summary>
    public interface IEmailTemplateRepository
    {
        /// <summary>
        /// Obtiene todas las plantillas de correo electrónico
        /// </summary>
        /// <returns>Lista de plantillas de correo electrónico</returns>
        Task<IEnumerable<EmailTemplate>> GetAllAsync();
        
        /// <summary>
        /// Obtiene una plantilla de correo electrónico por su ID
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        /// <returns>Plantilla de correo electrónico</returns>
        Task<EmailTemplate> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Obtiene una plantilla de correo electrónico por su nombre
        /// </summary>
        /// <param name="name">Nombre de la plantilla</param>
        /// <returns>Plantilla de correo electrónico</returns>
        Task<EmailTemplate> GetByNameAsync(string name);
        
        /// <summary>
        /// Añade una nueva plantilla de correo electrónico
        /// </summary>
        /// <param name="template">Plantilla de correo electrónico</param>
        Task AddAsync(EmailTemplate template);
        
        /// <summary>
        /// Actualiza una plantilla de correo electrónico existente
        /// </summary>
        /// <param name="template">Plantilla de correo electrónico</param>
        Task UpdateAsync(EmailTemplate template);
        
        /// <summary>
        /// Elimina una plantilla de correo electrónico
        /// </summary>
        /// <param name="id">ID de la plantilla</param>
        Task DeleteAsync(Guid id);
        
        /// <summary>
        /// Verifica si existe una plantilla con el nombre especificado
        /// </summary>
        /// <param name="name">Nombre de la plantilla</param>
        /// <returns>True si existe, False en caso contrario</returns>
        Task<bool> ExistsByNameAsync(string name);
    }
}
