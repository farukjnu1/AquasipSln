using Aquasip.EF;

namespace Aquasip.Models
{
    public class PurchaseReturnDetailVM
    {
        public long PurchaseReturnDetailId { get; set; }

        public long PurchaseReturnId { get; set; }

        public long ProductId { get; set; }

        public decimal? Qty { get; set; }

        public decimal? UnitCost { get; set; }

        public decimal? LineTotal { get; set; }

        public int? StoreId { get; set; }

        public bool? IsActive { get; set; }

        public virtual ProductVM Product { get; set; } = null!;

        public virtual PurchaseReturnVM PurchaseReturn { get; set; } = null!;
    }
}
