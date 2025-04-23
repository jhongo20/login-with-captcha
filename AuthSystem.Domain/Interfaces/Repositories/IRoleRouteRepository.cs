using AuthSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de RoleRoute
    /// </summary>
    public interface IRoleRouteRepository
    {
        /// <summary>
        /// Obtiene todas las rutas asignadas a un rol
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de rutas</returns>
        Task<IEnumerable<Route>> GetRoutesByRoleAsync(Guid roleId);

        /// <summary>
        /// Obtiene todos los roles que tienen acceso a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Lista de roles</returns>
        Task<IEnumerable<Role>> GetRolesByRouteAsync(Guid routeId);

        /// <summary>
        /// Asigna una ruta a un rol
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="roleId">ID del rol</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <returns>Task</returns>
        Task AssignRouteToRoleAsync(Guid routeId, Guid roleId, string userName);

        /// <summary>
        /// Revoca una ruta de un rol
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Task</returns>
        Task RevokeRouteFromRoleAsync(Guid routeId, Guid roleId);

        /// <summary>
        /// Verifica si un rol tiene acceso a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>True si el rol tiene acceso a la ruta, False en caso contrario</returns>
        Task<bool> RoleHasRouteAsync(Guid routeId, Guid roleId);
        
        /// <summary>
        /// Obtiene todas las rutas de un módulo específico que son accesibles por un rol
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="roleId">ID del rol</param>
        /// <returns>Lista de rutas</returns>
        Task<IEnumerable<Route>> GetRoutesByModuleAndRoleAsync(Guid moduleId, Guid roleId);
        
        /// <summary>
        /// Verifica si un rol tiene acceso a un módulo
        /// </summary>
        /// <param name="roleId">ID del rol</param>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>True si el rol tiene acceso al módulo, False en caso contrario</returns>
        Task<bool> RoleHasModuleAccessAsync(Guid roleId, Guid moduleId);
    }
}