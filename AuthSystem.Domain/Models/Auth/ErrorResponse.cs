using System.Collections.Generic;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para respuestas de error
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Mensaje de error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Lista de errores detallados
        /// </summary>
        public List<string> Errors { get; set; }

        /// <summary>
        /// Tiempo restante de bloqueo en segundos
        /// </summary>
        public int? LockoutRemainingSeconds { get; set; }
    }
}
