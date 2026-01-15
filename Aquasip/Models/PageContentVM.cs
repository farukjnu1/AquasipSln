using System.ComponentModel.DataAnnotations;

namespace Aquasip.Models
{
    public class PageContentVM
    {
        [Key]
        public int PageContentId { get; set; }

        public string Title { get; set; } = null!;

        public string SlugPage { get; set; } = null!;

        public string SlugPageContent { get; set; } = null!;

        public string? Header { get; set; }

        public string? Body { get; set; }

        public string? Footer { get; set; }

        public int? MediaId { get; set; }

        public bool? IsActive { get; set; }

        public int? UploadedBy { get; set; }

        public DateTime? UploadedAt { get; set; }
        public string FileName { get; set; } = null!;

        public string FilePath { get; set; } = null!;

        public IFormFile MediaFile { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
