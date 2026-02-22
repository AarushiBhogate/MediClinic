namespace MediClinic.Models.ModelViews
{
    public class DrugRequestListVM
    {
        public int DrugRequestId { get; set; }
        public string PhysicianName { get; set; }
        public string DrugsInfoText { get; set; }
        public DateTime? RequestDate { get; set; }
        public string RequestStatus { get; set; }
    }
}
