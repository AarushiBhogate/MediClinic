namespace MediClinic.Models.ModelViews
{
    public class PendingAppointmentVM
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string RequiredSpecialization { get; set; }
        public string Criticality { get; set; }
    }
}
