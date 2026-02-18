using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediClinic.Models;
using Microsoft.AspNetCore.Http;

namespace MediClinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AppointmentsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var mediClinicDbContext = _context.Appointments.Include(a => a.Patient);
            return View(await mediClinicDbContext.ToListAsync());
        }

        public IActionResult Index(string status)
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


        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public IActionResult Create(Appointment appointment)
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

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }
    }
}







