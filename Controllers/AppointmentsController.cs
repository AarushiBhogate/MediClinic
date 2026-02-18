using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediClinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AppointmentsController(MediClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string status, string search)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
                return RedirectToAction("Login", "User");

            var appointments = _context.Appointments
                .Where(a => a.PatientId == patientId);

            if (!string.IsNullOrEmpty(status))
                appointments = appointments.Where(a => a.ScheduleStatus == status);

            if (!string.IsNullOrEmpty(search))
                appointments = appointments.Where(a => a.Reason.Contains(search));

            return View(appointments.OrderByDescending(a => a.AppointmentDate).ToList());
        }

        public IActionResult Details(int id)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
                return RedirectToAction("Login", "User");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == id &&
                                     a.PatientId == patientId);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        public IActionResult Cancel(int id)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == id &&
                                     a.PatientId == patientId);

            if (appointment != null &&
                (appointment.ScheduleStatus == "Scheduled" ||
                 appointment.ScheduleStatus == "Pending") &&
                 appointment.AppointmentDate > DateTime.Now)
            {
                appointment.ScheduleStatus = "Cancelled";
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}







