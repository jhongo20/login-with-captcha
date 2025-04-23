using AuthSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de PermissionRoute
    /// </summary>
    public interface IPermissionRouteRepository
    {
        /// <summary>
        /// Obtiene todos los permisos de una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Lista de permisos</returns>
        Task<IEnumerable<Permission>> GetPermissionsByRouteAsync(Guid routeId);

        /// <summary>
        /// Obtiene todas las rutas que requieren un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Lista de rutas</returns>
        Task<IEnumerable<Route>> GetRoutesByPermissionAsync(Guid permissionId);

        /// <summary>
        /// Asigna un permiso a una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <returns>Task</returns>
        Task AssignPermissionToRouteAsync(Guid routeId, Guid permissionId, string userName);

        /// <summary>
        /// Revoca un permiso de una ruta
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Task</returns>
        Task RevokePermissionFromRouteAsync(Guid routeId, Guid permissionId);

        /// <summary>
        /// Verifica si una ruta requiere un permiso específico
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>True si la ruta requiere el permiso, False en caso contrario</returns>
        Task<bool> RouteRequiresPermissionAsync(Guid routeId, Guid permissionId);
    }
}
