using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediClinic.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int? PatientId { get; set; }

    public string? AdminNote { get; set; }

    public DateTime? AppointmentDate { get; set; }


    public string? Criticality { get; set; }

    public string? Reason { get; set; }

    public string? Note { get; set; }
    


    public string? ScheduleStatus { get; set; }

    public virtual Patient? Patient { get; set; }

    public string? RequiredSpecialization { get; set; }


    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    

}
