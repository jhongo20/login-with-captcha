using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de permisos de rol
    /// </summary>
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        /// <summary>
        /// Obtiene los permisos de un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos de rol</returns>
        Task<IEnumerable<RolePermission>> GetByRoleAsync(Guid roleId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene los roles que tienen un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista de permisos de rol</returns>
        Task<IEnumerable<RolePermission>> GetByPermissionAsync(Guid permissionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene un permiso de rol específico
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Permiso de rol encontrado o null</returns>
        Task<RolePermission> GetByRoleAndPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un rol tiene un permiso específico
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el rol tiene el permiso</returns>
        Task<bool> RoleHasPermissionAsync(Guid roleId, Guid permissionId, CancellationToken cancellationToken = default);
    }
}
