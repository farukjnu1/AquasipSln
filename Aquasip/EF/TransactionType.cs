using System;
using System.Collections.Generic;

namespace Aquasip.EF;

public partial class TransactionType
{
    public int TransactionTypeId { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }
}
