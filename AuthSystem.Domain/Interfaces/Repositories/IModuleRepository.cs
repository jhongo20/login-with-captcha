using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de módulos
    /// </summary>
    public interface IModuleRepository : IRepository<Module>
    {
        /// <summary>
        /// Obtiene todos los módulos principales (sin padre)
        /// </summary>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos principales</returns>
        Task<IEnumerable<Module>> GetRootModulesAsync(bool includeChildren = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todos los submódulos de un módulo padre
        /// </summary>
        /// <param name="parentId">ID del módulo padre</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de submódulos</returns>
        Task<IEnumerable<Module>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene todos los módulos habilitados
        /// </summary>
        /// <param name="includeChildren">Indica si se deben incluir los submódulos</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos habilitados</returns>
        Task<IEnumerable<Module>> GetEnabledModulesAsync(bool includeChildren = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un módulo tiene submódulos
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si tiene submódulos, False en caso contrario</returns>
        Task<bool> HasChildrenAsync(Guid moduleId, CancellationToken cancellationToken = default);
    }
}
