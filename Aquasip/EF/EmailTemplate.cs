using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class EmailTemplate
{
    public int TemplateId { get; set; }

    public string TemplateCode { get; set; } = null!;

    public string TemplateName { get; set; } = null!;

    public string SubjectTemplate { get; set; } = null!;

    public string BodyTemplate { get; set; } = null!;

    public bool IsHtml { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<EmailRecipient> EmailRecipients { get; set; } = new List<EmailRecipient>();
}
