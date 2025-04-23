using System;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que representa la relación entre un permiso y una ruta
    /// </summary>
    public class PermissionRoute
    {
        /// <summary>
        /// Identificador único
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del permiso
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Referencia al permiso
        /// </summary>
        public Permission Permission { get; set; }

        /// <summary>
        /// Identificador de la ruta
        /// </summary>
        public Guid RouteId { get; set; }

        /// <summary>
        /// Referencia a la ruta
        /// </summary>
        public Route Route { get; set; }

        /// <summary>
        /// Indica si la relación está activa
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Usuario que creó el registro
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Fecha de última modificación
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }

        /// <summary>
        /// Usuario que realizó la última modificación
        /// </summary>
        public string LastModifiedBy { get; set; }
    }
}
