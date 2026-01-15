using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Comment
{
    public int CommentId { get; set; }

    public int PageId { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorEmail { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Page Page { get; set; } = null!;
}
