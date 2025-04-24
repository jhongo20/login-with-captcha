using System;
using System.Collections.Generic;
using AuthSystem.Domain.Common;
using AuthSystem.Domain.Common.Enums;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que representa a un usuario del sistema
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// Nombre de usuario (único en el sistema)
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
        /// Hash de la contraseña del usuario
        /// </summary>
        public string PasswordHash { get; set; }

        /// <summary>
        /// Tipo de usuario (interno o externo)
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        /// Estado actual del usuario (Activo, Inactivo, Bloqueado, Suspendido, Eliminado)
        /// </summary>
        public UserStatus UserStatus { get; set; } = UserStatus.Active;

        /// <summary>
        /// Indica si el usuario está activo (compatible con versiones anteriores)
        /// </summary>
        public bool IsActive 
        { 
            get => UserStatus == UserStatus.Active;
            set => UserStatus = value ? UserStatus.Active : UserStatus.Inactive;
        }

        /// <summary>
        /// Número de teléfono del usuario
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Indica si el número de teléfono está confirmado
        /// </summary>
        public bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// Indica si el correo electrónico está confirmado
        /// </summary>
        public bool EmailConfirmed { get; set; }

        /// <summary>
        /// Indica si el bloqueo de cuenta está habilitado
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// Fecha hasta la que el usuario está bloqueado
        /// </summary>
        public DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Número de intentos fallidos de acceso
        /// </summary>
        public int AccessFailedCount { get; set; }

        /// <summary>
        /// Sello de seguridad (cambia cuando se modifican credenciales)
        /// </summary>
        public string? SecurityStamp { get; set; }

        /// <summary>
        /// Sello de concurrencia para control de acceso concurrente
        /// </summary>
        public string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Indica si la autenticación de dos factores está habilitada
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// Clave secreta para la autenticación de dos factores
        /// </summary>
        public string? TwoFactorSecretKey { get; set; }

        /// <summary>
        /// Código de recuperación para la autenticación de dos factores
        /// </summary>
        public string? TwoFactorRecoveryCode { get; set; }

        /// <summary>
        /// Token para restablecer la contraseña
        /// </summary>
        public string? PasswordResetToken { get; set; }

        /// <summary>
        /// Fecha de expiración del token para restablecer la contraseña
        /// </summary>
        public DateTime? PasswordResetTokenExpiry { get; set; }

        /// <summary>
        /// Fecha del último cambio de contraseña
        /// </summary>
        public DateTime? LastPasswordChangeAt { get; set; }

        /// <summary>
        /// Roles asignados al usuario
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public User()
        {
            UserRoles = new HashSet<UserRole>();
        }
    }
}
