using System.Collections.Generic;

namespace MediClinic.Models.ModelViews
{
    public class LandingVM
    {
        public List<SpecializationVM> Specializations { get; set; }
        public List<DoctorCardVM> TopDoctors { get; set; }
    }

    public class SpecializationVM
    {
        public string Name { get; set; }
        public int DoctorCount { get; set; }
    }

    public class DoctorCardVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }
        public int Experience { get; set; }
        public string ImageUrl { get; set; }
    }
}
