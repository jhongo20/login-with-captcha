using System;

namespace AuthSystem.Domain.Common
{
    /// <summary>
    /// Clase base para todas las entidades del sistema
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Identificador único de la entidad
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Fecha de creación de la entidad
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Usuario que creó la entidad
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Fecha de última modificación de la entidad
        /// </summary>
        public DateTime? LastModifiedAt { get; set; }

        /// <summary>
        /// Usuario que realizó la última modificación de la entidad
        /// </summary>
        public string? LastModifiedBy { get; set; }

        /// <summary>
        /// Indica si la entidad está activa
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
