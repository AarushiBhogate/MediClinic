namespace MediClinic.Models.ModelViews
{
    public class ChemistDashboardVM
    {
        public int PendingDrugRequests { get; set; }
        public int OrdersInProgress { get; set; }
        public int CompletedOrders { get; set; }
        public int LowStockItems { get; set; }
    }
}
