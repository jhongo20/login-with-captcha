using System;

namespace AuthSystem.Domain.Models.Roles
{
    /// <summary>
    /// DTO para información de un permiso
    /// </summary>
    public class PermissionDto
    {
        /// <summary>
        /// Identificador del permiso
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre del permiso
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Descripción del permiso
        /// </summary>
        public string Description { get; set; }
    }
}
