using Microsoft.AspNetCore.Mvc.Rendering;
namespace MediClinic.Models {

    public class PurchaseOrderVM
    {
        public int DrugRequestId { get; set; }

        public string? Pono { get; set; }

        public DateTime Podate { get; set; }

        public int SupplierId { get; set; }

        public List<PurchaseProductLineVM>? ProductLines { get; set; }

        public List<SelectListItem>? Suppliers { get; set; }

        public List<SelectListItem>? Drugs { get; set; }
    }

    public class PurchaseProductLineVM
    {
        public int DrugId { get; set; }
        public int Qty { get; set; }
        public string? Note { get; set; }
    }
}