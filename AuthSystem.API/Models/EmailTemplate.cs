using System;
using System.Collections.Generic;

namespace AuthSystem.API.Models;

public partial class EmailTemplate
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Subject { get; set; }

    public string HtmlContent { get; set; }

    public string TextContent { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }

    public DateTime LastModifiedAt { get; set; }

    public string LastModifiedBy { get; set; }
}
