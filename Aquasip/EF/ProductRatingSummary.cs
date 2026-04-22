using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class ProductRatingSummary
{
    public long ProductId { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalReviews { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Product Product { get; set; } = null!;
}
