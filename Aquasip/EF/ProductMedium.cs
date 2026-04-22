using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class ProductMedium
{
    public long ProductMediaId { get; set; }

    public long? ProductId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }
}
