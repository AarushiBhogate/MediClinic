namespace MediClinic.Models
{
    public class ScheduleCreateVM
    {
        public int AppointmentId { get; set; }

        // ✅ Change this
        public DateOnly ScheduleDate { get; set; }

        public string? ScheduleTime { get; set; }
    }
}
