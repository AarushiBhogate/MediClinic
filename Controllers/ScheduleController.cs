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

            var today = DateOnly.FromDateTime(DateTime.Today);

            var list = (from s in _context.Schedules
                        join a in _context.Appointments on s.AppointmentId equals a.AppointmentId
                        join p in _context.Patients on a.PatientId equals p.PatientId
                        where s.PhysicianId == physicianId.Value
                              && s.ScheduleDate.HasValue
                              && s.ScheduleDate.Value >= today
                        orderby s.ScheduleDate ascending
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


            ViewBag.PageType = "Upcoming";
            return View("MySchedule", list);
        }
        public IActionResult PreviousSchedule()
        {
            var role = HttpContext.Session.GetString("Role");
            var physicianId = HttpContext.Session.GetInt32("RoleReferenceID");

            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "User");

            if (role != "Physician")
                return RedirectToAction("AccessDenied", "User");

            if (physicianId == null)
                return RedirectToAction("Login", "User");

            var today = DateOnly.FromDateTime(DateTime.Today);

            var list = (from s in _context.Schedules
                        join a in _context.Appointments on s.AppointmentId equals a.AppointmentId
                        join p in _context.Patients on a.PatientId equals p.PatientId
                        where s.PhysicianId == physicianId.Value
                              && s.ScheduleDate < today
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

            ViewBag.PageType = "Previous";
            return View(list);
        }



    }
}
