using System;
using System.Collections.Generic;

namespace Aquasip.Models
{
    public class SalesReturnDetailVM
    {
        public long SalesReturnDetailId { get; set; }

        public long SalesReturnId { get; set; }

        public long ProductId { get; set; }

        public decimal? Qty { get; set; }

        public decimal? UnitPrice { get; set; }

        public decimal? LineTotal { get; set; }

        public int? StoreId { get; set; }

        public bool? IsActive { get; set; }

        public virtual ProductVM Product { get; set; } = null!;

        public virtual SalesReturnVM SalesReturn { get; set; } = null!;
        public string ReferenceToken { get; set; } = null!;
        public string TransactionTypeToken { get; set; } = null!;
        public bool IsStockUpdated { get; set; }
    }
}
    
