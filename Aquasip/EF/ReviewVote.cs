using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class ReviewVote
{
    public long VoteId { get; set; }

    public long ReviewId { get; set; }

    public long CustomerId { get; set; }

    public bool IsHelpful { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Review Review { get; set; } = null!;
}
