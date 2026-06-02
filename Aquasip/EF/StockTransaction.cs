using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class StockTransaction
{
    public long StockTransactionId { get; set; }

    public DateTime TransactionDate { get; set; }

    public long ProductId { get; set; }

    public int? TransactionTypeId { get; set; }

    public int? ReferenceTypeId { get; set; }

    public long? ReferenceId { get; set; }

    public decimal? QtyIn { get; set; }

    public decimal? QtyOut { get; set; }

    public decimal? UnitCost { get; set; }

    public int? StoreId { get; set; }

    public bool? IsActive { get; set; }

    public virtual Product Product { get; set; } = null!;
}
