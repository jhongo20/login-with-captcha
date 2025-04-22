using System;
using AuthSystem.Domain.Common;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que representa un módulo del sistema
    /// </summary>
    public class Module : BaseEntity
    {
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
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Fecha de actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Usuario que actualizó el módulo
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Módulo padre (si es un submódulo)
        /// </summary>
        public virtual Module Parent { get; set; }

        /// <summary>
        /// Submódulos
        /// </summary>
        public virtual ICollection<Module> Children { get; set; } = new List<Module>();

        /// <summary>
        /// Permisos asociados a este módulo
        /// </summary>
        public virtual ICollection<PermissionModule> PermissionModules { get; set; } = new List<PermissionModule>();

        /// <summary>
        /// Rutas asociadas a este módulo
        /// </summary>
        public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
    }
}
