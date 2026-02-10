using System;
using System.Collections.Generic;

namespace MediClinic.Models;

public partial class PurchaseProductLine
{
    public int PolineId { get; set; }

    public int? Poid { get; set; }

    public int? DrugId { get; set; }

    public int? SlNo { get; set; }

    public int? Qty { get; set; }

    public string? Note { get; set; }

    public virtual Drug? Drug { get; set; }

    public virtual PurchaseOrderHeader? Po { get; set; }
}
