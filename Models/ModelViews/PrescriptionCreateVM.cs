namespace MediClinic.Models
{
    public class PrescriptionCreateVM
    {
        public int PhysicianAdviceId { get; set; }
        public int ScheduleId { get; set; }

        public string? PatientName { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public string? ScheduleTime { get; set; }

        public int DrugId { get; set; }
        public string? Dosage { get; set; }
        public string? Prescription { get; set; }
    }
}
