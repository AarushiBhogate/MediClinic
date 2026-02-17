using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }

}
