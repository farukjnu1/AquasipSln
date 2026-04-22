using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class ReviewMedium
{
    public long MediaId { get; set; }

    public long ReviewId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string? MediaType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Review Review { get; set; } = null!;
}
