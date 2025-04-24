using System;

namespace AuthSystem.Domain.Common.Enums
{
    /// <summary>
    /// Enumeración que define los posibles estados de un usuario
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        /// Usuario activo, puede iniciar sesión y utilizar el sistema
        /// </summary>
        Active = 1,

        /// <summary>
        /// Usuario inactivo, no puede iniciar sesión hasta que sea activado
        /// </summary>
        Inactive = 2,

        /// <summary>
        /// Usuario bloqueado temporalmente por intentos fallidos de inicio de sesión
        /// </summary>
        Locked = 3,

        /// <summary>
        /// Usuario suspendido por un administrador
        /// </summary>
        Suspended = 4,

        /// <summary>
        /// Usuario marcado como eliminado (eliminación lógica)
        /// </summary>
        Deleted = 5
    }
}
