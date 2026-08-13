using System;
using System.Collections.Generic;

namespace Aquasip.LocalDbModels;

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
}
