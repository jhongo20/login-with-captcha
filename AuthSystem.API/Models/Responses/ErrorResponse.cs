namespace AuthSystem.API.Models.Responses
{
    /// <summary>
    /// Respuesta de error estándar
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Mensaje de error
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Código de error (opcional)
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Tiempo restante de bloqueo en segundos (para errores de bloqueo de cuenta)
        /// </summary>
        public int? LockoutRemainingSeconds { get; set; }

        /// <summary>
        /// Detalles adicionales del error (opcional)
        /// </summary>
        public object Details { get; set; }
    }
}
