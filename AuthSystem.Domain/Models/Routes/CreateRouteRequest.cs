using System;
using System.ComponentModel.DataAnnotations;

namespace AuthSystem.Domain.Models.Routes
{
    public class CreateRouteRequest
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string Name { get; set; }
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; }
        
        [Required(ErrorMessage = "La ruta es obligatoria")]
        [StringLength(200, ErrorMessage = "La ruta no puede exceder los 200 caracteres")]
        public string Path { get; set; }
        
        [Required(ErrorMessage = "El método HTTP es obligatorio")]
        [StringLength(10, ErrorMessage = "El método HTTP no puede exceder los 10 caracteres")]
        public string HttpMethod { get; set; }
        
        public int DisplayOrder { get; set; }
        
        public bool RequiresAuth { get; set; } = true;
        
        public bool IsEnabled { get; set; } = true;
        
        public bool IsActive { get; set; } = true;
        
        [Required(ErrorMessage = "El ID del módulo es obligatorio")]
        public Guid ModuleId { get; set; }
    }
}
