using Aquasip.EF;

namespace Aquasip.Models
{
    public class ReviewCommentVM
    {
        public long CommentId { get; set; }

        public long ReviewId { get; set; }

        public long CustomerId { get; set; }

        public string? CommentText { get; set; }

        public long? ParentCommentId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public virtual CustomerVM Customer { get; set; } = null!;

        public virtual ReviewVM Review { get; set; } = null!;
    }
}
