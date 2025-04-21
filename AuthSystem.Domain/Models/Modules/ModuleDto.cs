using System;
using System.Collections.Generic;

namespace AuthSystem.Domain.Models.Modules
{
    /// <summary>
    /// DTO para la entidad Module
    /// </summary>
    public class ModuleDto
    {
        /// <summary>
        /// ID del módulo
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del módulo
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descripción del módulo
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Ruta del módulo
        /// </summary>
        public string Route { get; set; }

        /// <summary>
        /// Icono del módulo
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Orden de visualización
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// ID del módulo padre (si es un submódulo)
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Indica si el módulo está habilitado
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Submódulos
        /// </summary>
        public List<ModuleDto> Children { get; set; } = new List<ModuleDto>();
    }
}
