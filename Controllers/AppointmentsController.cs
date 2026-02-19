using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace MediClinic.Controllers
{
    public class AppointmentsController : PatientBaseController
    {
        private readonly MediClinicDbContext _context;

        public AppointmentsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // =========================
        // PATIENT VIEW (Filtered)
        // =========================
        public IActionResult Index(string status)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
                return RedirectToAction("Login", "User");

            var appointments = _context.Appointments
                .Where(a => a.PatientId == patientId);

            if (!string.IsNullOrEmpty(status))
                appointments = appointments.Where(a => a.ScheduleStatus == status);

            return View(appointments.ToList());
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            int? patientId = HttpContext.Session.GetInt32("PatientId");

            if (patientId == null)
                return RedirectToAction("Login", "User");

            appointment.PatientId = patientId;
            appointment.ScheduleStatus = "Pending";

            _context.Appointments.Add(appointment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // CANCEL
        // =========================
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

        // =========================
        // DETAILS
        // =========================
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
    }
}
