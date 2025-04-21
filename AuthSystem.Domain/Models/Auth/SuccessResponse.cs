namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para respuestas exitosas
    /// </summary>
    public class SuccessResponse
    {
        /// <summary>
        /// Mensaje de éxito
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Datos adicionales (opcional)
        /// </summary>
        public object Data { get; set; }
    }
}
