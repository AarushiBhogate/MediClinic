using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediClinic.Models.ModelViews;


namespace MediClinic.Controllers
{
    public class PhysicianAdviceController : Controller
    {
        private readonly MediClinicDbContext _context;

        public PhysicianAdviceController(MediClinicDbContext context)
        {
            _context = context;
        }

        private int? GetPhysicianId()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role)) return null;
            if (role != "Physician") return null;

            return HttpContext.Session.GetInt32("RoleReferenceID");
        }

        // ==========================
        // 1) Advice & Prescription Index
        // ==========================
        public IActionResult Index()
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            // Pending = Confirmed schedules but no advice written
            var pending = _context.Schedules
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(s => s.PhysicianAdvices)
                .Where(s =>
                    s.PhysicianId == physicianId &&
                    s.ScheduleStatus == "Confirmed" &&
                    !s.PhysicianAdvices.Any()
                )
                .Select(s => new AdviceListVM
                {
                    ScheduleId = s.ScheduleId,
                    AppointmentId = s.AppointmentId ?? 0,
                    PatientId = s.Appointment.Patient.PatientId, // ✅

                    PatientName = s.Appointment.Patient.PatientName,
                    ScheduleDate = s.ScheduleDate,
                    ScheduleTime = s.ScheduleTime,
                    Criticality = s.Appointment.Criticality,
                    Reason = s.Appointment.Reason,
                    ScheduleStatus = s.ScheduleStatus
                })
                .ToList();

            // Completed = advice exists
            var completed = _context.Schedules
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(s => s.PhysicianAdvices)
                .Where(s =>
                    s.PhysicianId == physicianId &&
                    s.PhysicianAdvices.Any()
                )
                .Select(s => new AdviceListVM
                {
                    ScheduleId = s.ScheduleId,
                    AppointmentId = s.AppointmentId ?? 0,
                    PatientName = s.Appointment.Patient.PatientName,
                    ScheduleDate = s.ScheduleDate,
                    ScheduleTime = s.ScheduleTime,
                    Criticality = s.Appointment.Criticality,
                    Reason = s.Appointment.Reason,
                    ScheduleStatus = "Completed",
                    PhysicianAdviceId = s.PhysicianAdvices.Select(x => x.PhysicianAdviceId).FirstOrDefault()
                })
                .ToList();

            ViewBag.Pending = pending;
            ViewBag.Completed = completed;

            return View();
        }

        // ==========================
        // 2) Write Advice (GET)
        // ==========================
        [HttpGet]
        public IActionResult Write(int scheduleId)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var schedule = _context.Schedules
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(s => s.PhysicianAdvices)
                .FirstOrDefault(s => s.ScheduleId == scheduleId && s.PhysicianId == physicianId);

            if (schedule == null) return NotFound();

            if (schedule.PhysicianAdvices.Any())
                return RedirectToAction("ViewAdvice", new { scheduleId });

            ViewBag.Schedule = schedule;
            return View();
        }

        // ==========================
        // 3) Write Advice (POST)
        // ==========================
        [HttpPost]
        public IActionResult Write(int scheduleId, string advice, string note)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var schedule = _context.Schedules
                .Include(s => s.Appointment)
                .Include(s => s.PhysicianAdvices)
                .FirstOrDefault(s => s.ScheduleId == scheduleId && s.PhysicianId == physicianId);

            if (schedule == null) return NotFound();

            if (schedule.PhysicianAdvices.Any())
                return RedirectToAction("ViewAdvice", new { scheduleId });

            PhysicianAdvice pa = new PhysicianAdvice();
            pa.ScheduleId = scheduleId;
            pa.Advice = advice;
            pa.Note = note;

            _context.PhysicianAdvices.Add(pa);

            schedule.ScheduleStatus = "Completed";
            if (schedule.Appointment != null)
                schedule.Appointment.ScheduleStatus = "Completed";

            _context.SaveChanges();

            // Redirect to prescription page
            return RedirectToAction("Create", "Prescription", new { physicianAdviceId = pa.PhysicianAdviceId });
        }

        // ==========================
        // 4) View Advice
        // ==========================
        public IActionResult ViewAdvice(int scheduleId)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var schedule = _context.Schedules
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(s => s.PhysicianAdvices)
                .FirstOrDefault(s => s.ScheduleId == scheduleId && s.PhysicianId == physicianId);

            if (schedule == null) return NotFound();

            var advice = schedule.PhysicianAdvices.FirstOrDefault();
            if (advice == null)
                return RedirectToAction("Write", new { scheduleId });

            ViewBag.Schedule = schedule;
            ViewBag.Advice = advice;

            return View();
        }

        // ==========================
        // 5) Edit Advice (GET)
        // ==========================
        [HttpGet]
        public IActionResult EditAdvice(int scheduleId)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var schedule = _context.Schedules
                .Include(s => s.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(s => s.PhysicianAdvices)
                .FirstOrDefault(s => s.ScheduleId == scheduleId && s.PhysicianId == physicianId);

            if (schedule == null) return NotFound();

            var advice = schedule.PhysicianAdvices.FirstOrDefault();
            if (advice == null)
                return RedirectToAction("Write", new { scheduleId });

            ViewBag.Schedule = schedule;
            ViewBag.Advice = advice;

            return View();
        }

        // ==========================
        // 6) Edit Advice (POST)
        // ==========================
        [HttpPost]
        public IActionResult EditAdvice(int scheduleId, int physicianAdviceId, string advice, string note)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var schedule = _context.Schedules
                .FirstOrDefault(s => s.ScheduleId == scheduleId && s.PhysicianId == physicianId);

            if (schedule == null) return NotFound();

            var pa = _context.PhysicianAdvices
                .FirstOrDefault(x => x.PhysicianAdviceId == physicianAdviceId && x.ScheduleId == scheduleId);

            if (pa == null) return NotFound();

            pa.Advice = advice;
            pa.Note = note;

            _context.SaveChanges();

            return RedirectToAction("ViewAdvice", new { scheduleId });
        }
    }
}
