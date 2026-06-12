using Aquasip.EF;

namespace Aquasip.Models
{
    public partial class PurchaseOrderDetailVM
    {
        public long PurchaseOrderDetailId { get; set; }

        public long PurchaseOrderId { get; set; }

        public long ProductId { get; set; }

        public decimal? Qty { get; set; }

        public decimal? UnitCost { get; set; }

        public decimal? DiscountAmount { get; set; }

        public decimal? LineTotal { get; set; }

        public int? StoreId { get; set; }

        public bool? IsActive { get; set; }

        public virtual ProductVM Product { get; set; } = null!;

        public virtual PurchaseOrderVM PurchaseOrder { get; set; } = null!;
        public string ReferenceToken { get; set; } = null!;
        public string TransactionTypeToken { get; set; } = null!;
        public bool IsStockUpdated { get; set; }
    }
}
