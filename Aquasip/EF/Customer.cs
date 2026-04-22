using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Customer
{
    public long CustomerId { get; set; }

    public string CustomerCode { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ReviewComment> ReviewComments { get; set; } = new List<ReviewComment>();

    public virtual ICollection<ReviewVote> ReviewVotes { get; set; } = new List<ReviewVote>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
