using AuthSystem.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Repositories
{
    /// <summary>
    /// Interfaz para el repositorio de códigos de activación
    /// </summary>
    public interface IActivationCodeRepository : IGenericRepository<ActivationCode>
    {
        /// <summary>
        /// Obtiene un código de activación por su código
        /// </summary>
        /// <param name="code">Código de activación</param>
        /// <returns>Código de activación</returns>
        Task<ActivationCode> GetByCodeAsync(string code);

        /// <summary>
        /// Obtiene el código de activación activo más reciente para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Código de activación</returns>
        Task<ActivationCode> GetLatestActiveByUserIdAsync(Guid userId);

        /// <summary>
        /// Marca un código de activación como utilizado
        /// </summary>
        /// <param name="code">Código de activación</param>
        /// <param name="updatedBy">Usuario que actualiza el código</param>
        /// <returns>True si se actualizó correctamente, False en caso contrario</returns>
        Task<bool> MarkAsUsedAsync(string code, string updatedBy);
    }
}
