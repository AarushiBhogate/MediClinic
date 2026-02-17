using System;
using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models
{
    public partial class PatientMedicalProfile
    {
        [Key]
        public int MedicalProfileId { get; set; }

        public int PatientId { get; set; }

        public string? MedicalAllergies { get; set; }
        public string? MedicalPastIllness { get; set; }
        public string? MedicalChronicDiseases { get; set; }
        public string? MedicalNotes { get; set; }

        public virtual Patient? Patient { get; set; }
    }
}
