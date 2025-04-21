using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Permissions
{
    /// <summary>
    /// Modelo para la solicitud de creación de permiso
    /// </summary>
    public class CreatePermissionRequest
    {
        /// <summary>
        /// Nombre del permiso
        /// </summary>
        [Required(ErrorMessage = "El nombre del permiso es obligatorio")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre del permiso debe tener entre 3 y 50 caracteres")]
        public string Name { get; set; }

        /// <summary>
        /// Descripción del permiso
        /// </summary>
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string Description { get; set; }
    }
}
