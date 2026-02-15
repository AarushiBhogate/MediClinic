namespace MediClinic.Models.ModelViews
{
    public class PatientHistoryVM
    {
        public int ScheduleId { get; set; }
        public int PhysicianAdviceId { get; set; }

        public DateOnly? ScheduleDate { get; set; }
        public string? ScheduleTime { get; set; }

        public string? Reason { get; set; }
        public string? Criticality { get; set; }

        public string? Advice { get; set; }
        public string? Note { get; set; }
    }
}
