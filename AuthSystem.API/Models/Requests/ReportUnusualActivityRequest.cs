using System.ComponentModel.DataAnnotations;

namespace AuthSystem.API.Models.Requests
{
    /// <summary>
    /// Modelo para reportar actividad inusual en la cuenta de un usuario
    /// </summary>
    public class ReportUnusualActivityRequest
    {
        /// <summary>
        /// Tipo de actividad inusual detectada
        /// </summary>
        [Required(ErrorMessage = "El tipo de actividad es requerido")]
        public string ActivityType { get; set; }

        /// <summary>
        /// Dirección IP desde donde se realizó la actividad (opcional)
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// User-Agent del navegador (opcional)
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Ubicación geográfica aproximada (opcional)
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Información adicional sobre la actividad (opcional)
        /// </summary>
        public string AdditionalInfo { get; set; }
    }
}
