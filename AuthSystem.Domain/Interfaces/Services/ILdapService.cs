using System;
using System.Threading.Tasks;

namespace AuthSystem.Domain.Interfaces.Services
{
    /// <summary>
    /// Interfaz para el servicio de LDAP
    /// </summary>
    public interface ILdapService
    {
        /// <summary>
        /// Autentica un usuario contra el directorio LDAP
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña</param>
        /// <param name="organizationId">ID de la organización (opcional)</param>
        /// <returns>True si la autenticación es exitosa</returns>
        Task<bool> AuthenticateAsync(string username, string password, Guid? organizationId = null);

        /// <summary>
        /// Obtiene información de un usuario del directorio LDAP
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="organizationId">ID de la organización (opcional)</param>
        /// <returns>Información del usuario en formato de diccionario</returns>
        Task<System.Collections.Generic.Dictionary<string, string>> GetUserInfoAsync(string username, Guid? organizationId = null);
    }
}
