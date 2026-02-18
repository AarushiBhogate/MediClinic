using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediClinic.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly MediClinicDbContext _context;

        public ScheduleController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // PHYSICIAN: My Schedule
        // ==========================================
        public IActionResult MySchedule()
        {
            var role = HttpContext.Session.GetString("Role");
            var physicianId = HttpContext.Session.GetInt32("RoleReferenceID");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "User");

            if (role != "Physician")
                return RedirectToAction("AccessDenied", "User");

            if (physicianId == null)
                return RedirectToAction("Login", "User");

            // Fetch schedule list for that physician
            var list = (from s in _context.Schedules
                        join a in _context.Appointments on s.AppointmentId equals a.AppointmentId
                        join p in _context.Patients on a.PatientId equals p.PatientId
                        where s.PhysicianId == physicianId.Value
                        orderby s.ScheduleDate descending
                        select new MyScheduleVM
                        {
                            ScheduleId = s.ScheduleId,
                            ScheduleDate = s.ScheduleDate,
                            ScheduleTime = s.ScheduleTime,
                            ScheduleStatus = s.ScheduleStatus,

                            PatientName = p.PatientName,
                            Criticality = a.Criticality,
                            Reason = a.Reason
                        }).ToList();

            return View(list);
        }
    }
}
