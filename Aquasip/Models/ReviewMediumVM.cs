using Aquasip.EF;

namespace Aquasip.Models
{
    public class ReviewMediumVM
    {
        public long MediaId { get; set; }

        public long ReviewId { get; set; }

        public string MediaUrl { get; set; } = null!;

        public string? MediaType { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual ReviewVM Review { get; set; } = null!;
    }
}
