
namespace Aquasip.Models
{
    public class PageVM
    {
        public int PageId { get; set; }

        public string? Title { get; set; } = null!;

        public string? Slug { get; set; } = null!;

        public string? Content { get; set; }

        public int? AuthorId { get; set; }

        public string? Status { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? CreatedAt { get; set; }
        public List<PageContentVM> PageContents { get; set; } = new List<PageContentVM>();
        public List<SiteSettingVM> SiteSettings { get; set; } = new List<SiteSettingVM>();
        public List<ProductVM> Products { get; set; } = new List<ProductVM>();
        public List<GalleryVM> Galleries { get; set; } = new List<GalleryVM>();
    }
}
