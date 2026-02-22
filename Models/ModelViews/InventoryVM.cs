namespace MediClinic.Models.ModelViews
{
    public class InventoryVM
    {
        public string DrugTitle { get; set; }
        public string Dosage { get; set; }
        public int? StockQuantity { get; set; }
        public DateTime? Expiry { get; set; }
    }
}
