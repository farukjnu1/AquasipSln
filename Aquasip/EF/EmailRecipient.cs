using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class EmailRecipient
{
    public long RecipientId { get; set; }

    public int TemplateId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string? DisplayName { get; set; }

    public bool IsActive { get; set; }

    public virtual EmailTemplate Template { get; set; } = null!;
}
