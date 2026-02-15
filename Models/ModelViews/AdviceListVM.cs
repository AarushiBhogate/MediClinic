namespace MediClinic.Models
{
    public class AdviceListVM
    {
        public int ScheduleId { get; set; }
        public int AppointmentId { get; set; }
        public string? PatientName { get; set; }

        public int PatientId { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public string? ScheduleTime { get; set; }
        public string? Criticality { get; set; }
        public string? Reason { get; set; }
        public string? ScheduleStatus { get; set; }

        public int? PhysicianAdviceId { get; set; }
        public string? Advice { get; set; }
        public string? Note { get; set; }
    }
}
