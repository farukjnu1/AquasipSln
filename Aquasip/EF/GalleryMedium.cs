using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class GalleryMedium
{
    public int MediaId { get; set; }

    public int? GalleryId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Gallery? Gallery { get; set; }
}
