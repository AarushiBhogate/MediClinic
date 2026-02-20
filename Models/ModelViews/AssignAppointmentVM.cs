using System;
using System.Collections.Generic;
using MediClinic.Models;

namespace MediClinic.Models.ModelViews
{
    public class AssignAppointmentVM
    {
        public int AppointmentId { get; set; }

        public string? PatientName { get; set; }

        public DateTime? RequestedDate { get; set; }

        public string? RequiredSpecialization { get; set; }

        public List<Physician> AvailableDoctors { get; set; } = new();

        public int SelectedPhysicianId { get; set; }

        public DateOnly ConfirmedDate { get; set; }

        public string? ConfirmedTime { get; set; }



    }
}
