using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class ReviewComment
{
    public long CommentId { get; set; }

    public long ReviewId { get; set; }

    public long CustomerId { get; set; }

    public string? CommentText { get; set; }

    public long? ParentCommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Review Review { get; set; } = null!;
}
