using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthSystem.Domain.Entities;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    public interface IRouteRepository : IGenericRepository<Route>
    {
        /// <summary>
        /// Obtiene todas las rutas habilitadas
        /// </summary>
        Task<IEnumerable<Route>> GetEnabledRoutesAsync();
        
        /// <summary>
        /// Obtiene todas las rutas de un módulo específico
        /// </summary>
        Task<IEnumerable<Route>> GetRoutesByModuleAsync(Guid moduleId);
        
        /// <summary>
        /// Obtiene todas las rutas asignadas a un rol específico
        /// </summary>
        Task<IEnumerable<Route>> GetRoutesByRoleAsync(Guid roleId);
        
        /// <summary>
        /// Verifica si un rol tiene acceso a una ruta específica
        /// </summary>
        Task<bool> RoleHasRouteAccessAsync(Guid roleId, Guid routeId);
        
        /// <summary>
        /// Verifica si un rol tiene acceso al módulo de una ruta
        /// </summary>
        Task<bool> RoleHasModuleAccessAsync(Guid roleId, Guid moduleId);
        
        /// <summary>
        /// Verifica si ya existe una ruta con el mismo nombre en el mismo módulo
        /// </summary>
        Task<bool> ExistsWithNameInModuleAsync(string name, Guid moduleId, Guid? excludeRouteId = null);
        
        /// <summary>
        /// Verifica si ya existe una ruta con el mismo path y método HTTP
        /// </summary>
        Task<bool> ExistsWithPathAndMethodAsync(string path, string httpMethod, Guid? excludeRouteId = null);
        
        /// <summary>
        /// Asigna una ruta a un rol
        /// </summary>
        Task AssignRouteToRoleAsync(Guid routeId, Guid roleId, string userName);
        
        /// <summary>
        /// Revoca el acceso de un rol a una ruta
        /// </summary>
        Task RevokeRouteFromRoleAsync(Guid routeId, Guid roleId);

        /// <summary>
        /// Obtiene todas las rutas de un módulo específico a las que tiene acceso un rol
        /// </summary>
        Task<IEnumerable<Route>> GetRoutesByModuleAndRoleAsync(Guid moduleId, Guid roleId);
    }
}
