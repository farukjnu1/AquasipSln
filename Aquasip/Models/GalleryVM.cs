using System;
using System.Collections.Generic;

namespace Aquasip.Models;

public class GalleryVM
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

    public virtual ICollection<GalleryMediumVM> GalleryMedia { get; set; } = new List<GalleryMediumVM>();
    public IFormFile? MediaFile { get; set; }
    public List<GalleryMediumVM> ListGalleryMedia { get; set; } = new List<GalleryMediumVM>();
    public string? Medias { get; set; }
}
