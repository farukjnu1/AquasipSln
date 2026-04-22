using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class ProductPrice
{
    public long ProductPriceId { get; set; }

    public long ProductId { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }
}
