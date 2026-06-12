using Aquasip.EF;

namespace Aquasip.Models
{
    public partial class PaymentMethodVM
    {
        public int PaymentMethodId { get; set; }

        public string PaymentMethodName { get; set; } = null!;

        public bool IsActive { get; set; }

        //public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        //public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }

}
