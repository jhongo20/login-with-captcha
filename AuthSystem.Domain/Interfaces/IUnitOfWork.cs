using System;
using System.Threading;
using System.Threading.Tasks;
using AuthSystem.Domain.Interfaces.Repositories;

namespace AuthSystem.Domain.Interfaces
{
    /// <summary>
    /// Interfaz para la unidad de trabajo
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Repositorio de usuarios
        /// </summary>
        IUserRepository Users { get; }

        /// <summary>
        /// Repositorio de roles
        /// </summary>
        IRoleRepository Roles { get; }

        /// <summary>
        /// Repositorio de permisos
        /// </summary>
        IPermissionRepository Permissions { get; }

        /// <summary>
        /// Repositorio de roles de usuario
        /// </summary>
        IUserRoleRepository UserRoles { get; }

        /// <summary>
        /// Repositorio de permisos de rol
        /// </summary>
        IRolePermissionRepository RolePermissions { get; }

        /// <summary>
        /// Repositorio de sesiones de usuario
        /// </summary>
        IUserSessionRepository UserSessions { get; }

        /// <summary>
        /// Repositorio de módulos
        /// </summary>
        IModuleRepository Modules { get; }

        /// <summary>
        /// Repositorio de rutas
        /// </summary>
        IRouteRepository Routes { get; }
        
        /// <summary>
        /// Repositorio de relaciones entre roles y rutas
        /// </summary>
        IRoleRouteRepository RoleRoutes { get; }
        
        /// <summary>
        /// Repositorio de relaciones entre permisos y módulos
        /// </summary>
        IPermissionModuleRepository PermissionModules { get; }
        
        /// <summary>
        /// Repositorio de relaciones entre permisos y rutas
        /// </summary>
        IPermissionRouteRepository PermissionRoutes { get; }

        /// <summary>
        /// Guarda los cambios en la base de datos
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Número de entidades modificadas</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Inicia una transacción
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Confirma la transacción actual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Revierte la transacción actual
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Task</returns>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
