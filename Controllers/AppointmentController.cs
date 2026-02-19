using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AppointmentController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // PHYSICIAN: Appointment Requests
        // ==========================================
        //public IActionResult Requests()
        //{
        //    var role = HttpContext.Session.GetString("Role");
        //    var physicianId = HttpContext.Session.GetInt32("RoleReferenceID");

        //    if (string.IsNullOrEmpty(role))
        //        return RedirectToAction("Login", "User");

        //    if (role != "Physician")
        //        return RedirectToAction("AccessDenied", "User");

        //    if (physicianId == null)
        //        return RedirectToAction("Login", "User");

        //    // Get physician specialization
        //    var physician = _context.Physicians.FirstOrDefault(p => p.PhysicianId == physicianId);

        //    if (physician == null)
        //        return Content("Physician not found.");

        //    // Appointment Requests = Pending + not scheduled
        //    var requests = (from a in _context.Appointments
        //                    join p in _context.Patients on a.PatientId equals p.PatientId
        //                    where a.ScheduleStatus == "Pending"
        //                       && a.RequiredSpecialization == physician.Specialization
        //                    orderby a.AppointmentDate descending
        //                    select new AppointmentRequestVM
        //                    {
        //                        AppointmentId = a.AppointmentId,
        //                        PatientName = p.PatientName,
        //                        AppointmentDate = a.AppointmentDate,
        //                        Criticality = a.Criticality,
        //                        Reason = a.Reason,
        //                        Note = a.Note,
        //                        RequiredSpecialization = a.RequiredSpecialization
        //                    }).ToList();

        //    ViewBag.Specialization = physician.Specialization;
        //    return View(requests);
        //}

        // ==========================================
        // PHYSICIAN: Accept & Schedule Appointment
        // ==========================================
        //[HttpGet]
        //public IActionResult Schedule(int id)
        //{
        //    var role = HttpContext.Session.GetString("Role");
        //    var physicianId = HttpContext.Session.GetInt32("RoleReferenceID");

        //    if (string.IsNullOrEmpty(role))
        //        return RedirectToAction("Login", "User");

        //    if (role != "Physician")
        //        return RedirectToAction("AccessDenied", "User");

        //    if (physicianId == null)
        //        return RedirectToAction("Login", "User");

        //    var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == id);

        //    if (appointment == null)
        //        return NotFound();

        //    var vm = new ScheduleCreateVM
        //    {
        //        AppointmentId = appointment.AppointmentId,
        //        ScheduleDate = DateOnly.FromDateTime(DateTime.Now),
        //        ScheduleTime = "10:00"
        //    };

        //    return View(vm);
        //}

        //[HttpPost]
        //public IActionResult Schedule(ScheduleCreateVM vm)
        //{
        //    var role = HttpContext.Session.GetString("Role");
        //    var physicianId = HttpContext.Session.GetInt32("RoleReferenceID");

        //    if (string.IsNullOrEmpty(role))
        //        return RedirectToAction("Login", "User");

        //    if (role != "Physician")
        //        return RedirectToAction("AccessDenied", "User");

        //    if (physicianId == null)
        //        return RedirectToAction("Login", "User");

        //    // Insert schedule
        //    Schedule schedule = new Schedule();
        //    schedule.PhysicianId = physicianId.Value;
        //    schedule.AppointmentId = vm.AppointmentId;
        //    schedule.ScheduleDate = vm.ScheduleDate;

        //    schedule.ScheduleTime = vm.ScheduleTime;
        //    schedule.ScheduleStatus = "Confirmed";

        //    _context.Schedules.Add(schedule);

        //    // Update appointment status
        //    var appointment = _context.Appointments.FirstOrDefault(a => a.AppointmentId == vm.AppointmentId);
        //    if (appointment != null)
        //    {
        //        appointment.ScheduleStatus = "Scheduled";
        //    }

        //    _context.SaveChanges();

        //    return RedirectToAction("Requests");
        //}
    }
}
