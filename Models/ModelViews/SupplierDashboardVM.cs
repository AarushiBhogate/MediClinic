namespace MediClinic.Models.ModelViews
{
    public class SupplierDashboardVM
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ApprovedOrders { get; set; }
        public int DispatchedOrders { get; set; }
        public int DeliveredOrders { get; set; }
    }

}
