using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Product
{
    public long ProductId { get; set; }

    public string ProductCode { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductRatingSummary? ProductRatingSummary { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
