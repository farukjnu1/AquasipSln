using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class Store
{
    public int StoreId { get; set; }

    public string? StoreCode { get; set; }

    public string? StoreName { get; set; }

    public bool? IsActive { get; set; }
}
