namespace MediClinic.Models.ModelViews
{
    public class PurchaseOrderListVM
    {
        public int Poid { get; set; }
        public string Pono { get; set; }
        public string SupplierName { get; set; }
        public DateTime? Podate { get; set; }
        public int LineCount { get; set; }
        public string Status { get; set; }
        public string Source { get; set; }
    }
}