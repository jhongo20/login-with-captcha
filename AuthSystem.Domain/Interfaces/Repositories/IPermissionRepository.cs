using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de permisos
    /// </summary>
    public interface IPermissionRepository : IRepository<Permission>
    {
        /// <summary>
        /// Obtiene un permiso por su nombre
        /// </summary>
        /// <param name="name">Nombre del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Permiso encontrado o null</returns>
        Task<Permission> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un nombre de permiso ya existe
        /// </summary>
        /// <param name="name">Nombre del permiso</param>
        /// <param name="excludePermissionId">ID de permiso a excluir (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el nombre de permiso ya existe</returns>
        Task<bool> NameExistsAsync(string name, Guid? excludePermissionId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene los permisos de un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos del rol</returns>
        Task<IEnumerable<Permission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene los permisos de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos del usuario</returns>
        Task<IEnumerable<Permission>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
