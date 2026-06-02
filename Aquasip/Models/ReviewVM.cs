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
        public string? CreatedAtPast { get; set; }

        public bool? IsDeleted { get; set; }

        public string? ModerationStatus { get; set; }
        public string? Attaches { get; set; }

        public virtual CustomerVM Customer { get; set; } = null!;

        public virtual ProductVM Product { get; set; } = null!;

        public virtual ICollection<ReviewCommentVM> ReviewComments { get; set; } = new List<ReviewCommentVM>();

        public virtual ICollection<ReviewMediumVM> ReviewMedia { get; set; } = new List<ReviewMediumVM>();

        public virtual ICollection<ReviewVoteVM> ReviewVotes { get; set; } = new List<ReviewVoteVM>();
        public List<IFormFile> Attachments { get; set; } = new List<IFormFile>();
        public int Helpful { get; set; }
        public int NotHelpful { get; set; }
        public enum QueryType
        {
            GetAll = 0, GetById = 1, Insert = 2, Update = 3, Delete = 4
        }
    }
}
