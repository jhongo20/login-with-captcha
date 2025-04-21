using System;
using AuthSystem.Domain.Common;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que representa una sesión de usuario
    /// </summary>
    public class UserSession : BaseEntity
    {
        /// <summary>
        /// ID del usuario
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Token de actualización (refresh token)
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Fecha de expiración del token de actualización
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Dirección IP desde la que se inició sesión
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Información del dispositivo desde el que se inició sesión
        /// </summary>
        public string DeviceInfo { get; set; }

        /// <summary>
        /// Indica si la sesión está activa
        /// </summary>
        public new bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha de la última actividad
        /// </summary>
        public DateTime LastActivity { get; set; }

        /// <summary>
        /// Usuario relacionado
        /// </summary>
        public virtual User User { get; set; }
    }
}
