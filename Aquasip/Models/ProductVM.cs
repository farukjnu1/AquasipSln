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

        public decimal? AverageRating { get; set; }

        public int? TotalReviews { get; set; }

        public List<ReviewVM> ListReview { get; set; } = new List<ReviewVM>();
        public string? Reviews { get; set; }
        public IFormFile? MediaFile { get; set; }
        public List<ProductMediumVM> ListProductMedia { get; set; } = new List<ProductMediumVM>();
        public string? Medias { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Total { get; set; }
    }
}
