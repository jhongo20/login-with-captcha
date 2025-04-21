using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de roles
    /// </summary>
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>
        /// Obtiene un rol por su nombre
        /// </summary>
        /// <param name="name">Nombre del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Rol encontrado o null</returns>
        Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un nombre de rol ya existe
        /// </summary>
        /// <param name="name">Nombre del rol</param>
        /// <param name="excludeRoleId">ID de rol a excluir (opcional)</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el nombre de rol ya existe</returns>
        Task<bool> NameExistsAsync(string name, Guid? excludeRoleId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene los roles de un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de roles del usuario</returns>
        Task<IEnumerable<Role>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
