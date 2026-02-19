using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MediClinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AppointmentsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public IActionResult Index(string? status, string? search)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
                return RedirectToAction("Login", "User");

            var appointments = _context.Appointments
                .Where(a => a.PatientId == patientId);

            if (!string.IsNullOrEmpty(status))
                appointments = appointments
                    .Where(a => a.ScheduleStatus == status);

            if (!string.IsNullOrEmpty(search))
                appointments = appointments
                    .Where(a => a.Reason.Contains(search));

            return View(appointments.ToList());
        }

        // ================= CREATE (GET) =================
        public IActionResult Create()
        {
            return View();
        }

        // ================= CREATE (POST) =================
        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
                return RedirectToAction("Login", "User");

            if (!ModelState.IsValid)
                return View(appointment);

            appointment.PatientId = patientId.Value;
            appointment.ScheduleStatus = "Pending";

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================= CANCEL =================
        public IActionResult Cancel(int id)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
                return RedirectToAction("Login", "User");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == id &&
                                     a.PatientId == patientId);

            if (appointment == null)
                return NotFound();

            if ((appointment.ScheduleStatus == "Pending" ||
                 appointment.ScheduleStatus == "Scheduled") &&
                 appointment.AppointmentDate > DateTime.Now)
            {
                appointment.ScheduleStatus = "Cancelled";
                _context.SaveChanges();
            }


            return RedirectToAction("Index");
        }
    }
}
