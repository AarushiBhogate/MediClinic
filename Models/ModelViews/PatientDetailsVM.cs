namespace MediClinic.Models.ModelViews
{
    public class PatientDetailsVM
    {
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }

        public string? Allergies { get; set; }
        public string? PastIllness { get; set; }
        public string? ChronicDiseases { get; set; }
        public string? Notes { get; set; }

        public List<PatientHistoryVM> PreviousVisits { get; set; } = new();
    }
}
