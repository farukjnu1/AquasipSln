using Aquasip.EF;

namespace Aquasip.Models
{
    public class ReviewVM
    {
        public long ReviewId { get; set; }

        public long ProductId { get; set; }

        public long CustomerId { get; set; }

        public string? Title { get; set; }

        public string? ReviewText { get; set; }

        public int Rating { get; set; }

        public bool? IsApproved { get; set; }

        public DateTime? CreatedAt { get; set; }

        public bool? IsDeleted { get; set; }

        public string? ModerationStatus { get; set; }

        public virtual Customer Customer { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;

        public virtual ICollection<ReviewComment> ReviewComments { get; set; } = new List<ReviewComment>();

        public virtual ICollection<ReviewMedium> ReviewMedia { get; set; } = new List<ReviewMedium>();

        public virtual ICollection<ReviewVote> ReviewVotes { get; set; } = new List<ReviewVote>();
    }
}
