using System;

namespace AuthSystem.Domain.Entities
{
    /// <summary>
    /// Entidad que representa un código de activación para un usuario
    /// </summary>
    public class ActivationCode
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ActivationCode()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            IsUsed = false;
            CreatedBy = "System";
        }

        /// <summary>
        /// Identificador único del código de activación
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del usuario al que pertenece el código
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Código de activación
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Fecha de expiración del código
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Indica si el código ha sido utilizado
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        /// Fecha de creación del código
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Usuario que creó el código
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de actualización del código
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Usuario que actualizó el código
        /// </summary>
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Usuario al que pertenece el código
        /// </summary>
        public virtual User? User { get; set; }
    }
}
