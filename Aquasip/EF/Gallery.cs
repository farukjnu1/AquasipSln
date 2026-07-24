using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Gallery
{
    public int GalleryId { get; set; }

    public string Code { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Header { get; set; }

    public string? Body { get; set; }

    public string? Footer { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual ICollection<GalleryMedium> GalleryMedia { get; set; } = new List<GalleryMedium>();
}
