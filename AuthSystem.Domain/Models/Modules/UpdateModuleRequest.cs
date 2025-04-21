using System;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Modules
{
    /// <summary>
    /// DTO para actualizar un módulo
    /// </summary>
    public class UpdateModuleRequest
    {
        /// <summary>
        /// Nombre del módulo
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        public string Name { get; set; }

        /// <summary>
        /// Descripción del módulo
        /// </summary>
        [StringLength(200, ErrorMessage = "La descripción no puede tener más de 200 caracteres")]
        public string Description { get; set; }

        /// <summary>
        /// Ruta del módulo
        /// </summary>
        [StringLength(100, ErrorMessage = "La ruta no puede tener más de 100 caracteres")]
        public string Route { get; set; }

        /// <summary>
        /// Icono del módulo
        /// </summary>
        [StringLength(50, ErrorMessage = "El icono no puede tener más de 50 caracteres")]
        public string Icon { get; set; }

        /// <summary>
        /// Orden de visualización
        /// </summary>
        [Range(0, 1000, ErrorMessage = "El orden debe estar entre 0 y 1000")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// ID del módulo padre (si es un submódulo)
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Indica si el módulo está habilitado
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
