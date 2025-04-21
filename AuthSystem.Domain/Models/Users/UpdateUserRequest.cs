using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Auth
{
    /// <summary>
    /// Modelo para la solicitud de actualización de usuario
    /// </summary>
    public class UpdateUserRequest
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres")]
        public string Username { get; set; }

        /// <summary>
        /// Correo electrónico
        /// </summary>
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo electrónico no puede tener más de 100 caracteres")]
        public string Email { get; set; }

        /// <summary>
        /// Nombre completo
        /// </summary>
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre completo debe tener entre 3 y 100 caracteres")]
        public string FullName { get; set; }

        /// <summary>
        /// Contraseña
        /// </summary>
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$", 
            ErrorMessage = "La contraseña debe contener al menos una letra minúscula, una letra mayúscula, un número y un carácter especial")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmación de contraseña
        /// </summary>
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación de contraseña no coinciden")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Indica si el usuario está activo
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Roles del usuario
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
