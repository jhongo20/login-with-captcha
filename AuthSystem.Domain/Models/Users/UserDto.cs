using System;
using AuthSystem.Domain.Common.Enums;

namespace AuthSystem.Domain.Models.Users
{
    /// <summary>
    /// DTO para información completa del usuario
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// Identificador del usuario
        /// </summary>
        public Guid Id { get; set; }

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
        /// Indica si el usuario está activo
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Estado del usuario
        /// </summary>
        public UserStatus UserStatus { get; set; }

        /// <summary>
        /// Nombre descriptivo del estado del usuario
        /// </summary>
        public string StatusName => UserStatus.ToString();

        /// <summary>
        /// Tipo de usuario
        /// </summary>
        public string UserType { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }

        /// <summary>
        /// Roles del usuario
        /// </summary>
        public string[] Roles { get; set; }
    }
}
