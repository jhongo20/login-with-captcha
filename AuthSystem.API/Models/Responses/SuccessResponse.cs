namespace AuthSystem.API.Models.Responses
{
    /// <summary>
    /// Respuesta de éxito estándar
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
