namespace MediClinic.Models
{
    public class AppointmentRequestVM
    {
        public int AppointmentId { get; set; }
        public string? PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string? Criticality { get; set; }
        public string? Reason { get; set; }
        public string? Note { get; set; }
        public string? RequiredSpecialization { get; set; }
    }
}
