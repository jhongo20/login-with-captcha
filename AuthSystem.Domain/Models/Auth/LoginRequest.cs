using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para la solicitud de inicio de sesión
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public string Password { get; set; }

        /// <summary>
        /// Indica si el usuario es de LDAP
        /// </summary>
        public bool IsLdapUser { get; set; } = false;
    }
}
