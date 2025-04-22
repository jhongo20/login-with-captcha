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

        /// <summary>
        /// Obtiene todos los módulos asignados a un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de módulos asignados al rol</returns>
        Task<IEnumerable<Module>> GetModulesByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un rol tiene acceso a un módulo
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el rol tiene acceso al módulo, False en caso contrario</returns>
        Task<bool> RoleHasModuleAccessAsync(Guid roleId, Guid moduleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asigna un módulo a un rol
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tarea completada</returns>
        Task AssignModuleToRoleAsync(Guid moduleId, Guid roleId, string userName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revoca el acceso de un rol a un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Tarea completada</returns>
        Task RevokeModuleFromRoleAsync(Guid moduleId, Guid roleId, CancellationToken cancellationToken = default);
    }
}
