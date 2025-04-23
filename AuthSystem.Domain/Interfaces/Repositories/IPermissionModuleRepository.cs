using AuthSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de PermissionModule
    /// </summary>
    public interface IPermissionModuleRepository
    {
        /// <summary>
        /// Obtiene todos los permisos de un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <returns>Lista de permisos</returns>
        Task<IEnumerable<Permission>> GetPermissionsByModuleAsync(Guid moduleId);

        /// <summary>
        /// Obtiene todos los módulos que requieren un permiso específico
        /// </summary>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Lista de módulos</returns>
        Task<IEnumerable<Module>> GetModulesByPermissionAsync(Guid permissionId);

        /// <summary>
        /// Asigna un permiso a un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <param name="userName">Nombre del usuario que realiza la asignación</param>
        /// <returns>Task</returns>
        Task AssignPermissionToModuleAsync(Guid moduleId, Guid permissionId, string userName);

        /// <summary>
        /// Revoca un permiso de un módulo
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>Task</returns>
        Task RevokePermissionFromModuleAsync(Guid moduleId, Guid permissionId);

        /// <summary>
        /// Verifica si un módulo requiere un permiso específico
        /// </summary>
        /// <param name="moduleId">ID del módulo</param>
        /// <param name="permissionId">ID del permiso</param>
        /// <returns>True si el módulo requiere el permiso, False en caso contrario</returns>
        Task<bool> ModuleRequiresPermissionAsync(Guid moduleId, Guid permissionId);
    }
}
