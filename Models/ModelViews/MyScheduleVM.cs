namespace MediClinic.Models
{
    public class MyScheduleVM
    {
        public int ScheduleId { get; set; }

        public DateOnly? ScheduleDate { get; set; }
        public string? ScheduleTime { get; set; }
        public string? ScheduleStatus { get; set; }

        public string? PatientName { get; set; }
        public string? Criticality { get; set; }
        public string? Reason { get; set; }
    }
}
