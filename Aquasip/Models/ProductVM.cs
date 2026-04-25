using Aquasip.EF;

namespace Aquasip.Models
{
    public class ProductVM
    {
        public long ProductId { get; set; }

        public string ProductCode { get; set; } = null!;

        public string ProductName { get; set; } = null!;

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public bool? IsActive { get; set; }

        public int? UploadedBy { get; set; }

        public DateTime? UploadedAt { get; set; }

        public virtual ProductRatingSummary? ProductRatingSummary { get; set; }

        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public IFormFile MediaFile { get; set; }
        public List<ProductMediumVM> listProductMedia = new List<ProductMediumVM>();

    }
}
