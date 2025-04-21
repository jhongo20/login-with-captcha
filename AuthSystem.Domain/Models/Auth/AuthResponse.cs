using System;
using AuthSystem.Domain.Models.Users;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para la respuesta de autenticación
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Correo electrónico del usuario
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Nombre completo del usuario
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Token JWT
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Token de actualización
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Fecha de expiración del token
        /// </summary>
        public DateTime TokenExpiration { get; set; }

        /// <summary>
        /// Indica si el usuario es de LDAP
        /// </summary>
        public bool IsLdapUser { get; set; }

        /// <summary>
        /// Roles del usuario
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// Permisos del usuario
        /// </summary>
        public string[] Permissions { get; set; }

        /// <summary>
        /// Tiempo de expiración en segundos
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Tipo de token
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Información del usuario
        /// </summary>
        public UserDto User { get; set; }
    }
}
