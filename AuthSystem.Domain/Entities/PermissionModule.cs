using System;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que relaciona permisos con módulos
    /// </summary>
    public class PermissionModule
    {
        /// <summary>
        /// ID de la relación
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID del permiso
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// Permiso relacionado
        /// </summary>
        public Permission Permission { get; set; }

        /// <summary>
        /// ID del módulo
        /// </summary>
        public Guid ModuleId { get; set; }

        /// <summary>
        /// Módulo relacionado
        /// </summary>
        public Module Module { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Usuario que creó la relación
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
