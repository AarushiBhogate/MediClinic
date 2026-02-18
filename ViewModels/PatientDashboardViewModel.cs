using MediClinic.Models;

namespace MediClinic.ViewModels
{
    public class PatientDashboardViewModel
    {
        public Patient Patient { get; set; }
        public Appointment NextAppointment { get; set; }
        public int TotalPrescriptions { get; set; }
        public int PendingDrugRequests { get; set; }
    }
}





