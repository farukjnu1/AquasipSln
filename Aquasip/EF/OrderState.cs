using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class OrderState
{
    public int OrderStateId { get; set; }

    public string OrderStatus { get; set; } = null!;

    public string? Remark { get; set; }

    public int? Sequence { get; set; }

    public bool? IsActive { get; set; }
}
