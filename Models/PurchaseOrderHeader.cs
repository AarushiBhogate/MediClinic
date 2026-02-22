using System;
using System.Collections.Generic;

namespace MediClinic.Models;

public partial class PurchaseOrderHeader
{
    public int Poid { get; set; }

    public string? Pono { get; set; }

    public DateTime? Podate { get; set; }

    public int? SupplierId { get; set; }

    public string? PoStatus { get; set; }

    public int? DrugRequestId { get; set; }   // ✅ ADD THIS

    public virtual Supplier? Supplier { get; set; }

    public virtual DrugRequest? DrugRequest { get; set; }  // ✅ ADD THIS

    public virtual ICollection<PurchaseProductLine> PurchaseProductLines { get; set; } = new List<PurchaseProductLine>();
}