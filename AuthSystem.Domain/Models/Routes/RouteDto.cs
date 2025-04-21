using System;

namespace AuthSystem.Domain.Models.Routes
{
    public class RouteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public string HttpMethod { get; set; }
        public int DisplayOrder { get; set; }
        public bool RequiresAuth { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsActive { get; set; }
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
