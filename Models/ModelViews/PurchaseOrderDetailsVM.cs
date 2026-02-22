namespace MediClinic.Models.ModelViews
{
    public class PurchaseOrderDetailsVM
    {
        public string Pono { get; set; }
        public DateTime? Podate { get; set; }
        public string SupplierName { get; set; }
        public int Poid { get; set; }
        public List<PurchaseOrderLineVM> Lines { get; set; }
    }

    public class PurchaseOrderLineVM
    {
        public string DrugName { get; set; }
        public int? Qty { get; set; }
        public string Note { get; set; }
    }
}