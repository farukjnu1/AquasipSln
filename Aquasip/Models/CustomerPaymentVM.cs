using Aquasip.EF;

namespace Aquasip.Models
{
    public class CustomerPaymentVM
    {
        public long PaymentTransactionId { get; set; }

        public long OrderId { get; set; }

        public int PaymentMethodId { get; set; }

        public string? TransactionNumber { get; set; }

        public decimal PaidAmount { get; set; }

        public int PaymentStatusId { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? Remarks { get; set; }

        public virtual SalesOrderVM Order { get; set; } = null!;

        //public virtual PaymentMethod PaymentMethod { get; set; } = null!;

        //public virtual PaymentStatus PaymentStatus { get; set; } = null!;
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
    }

}
