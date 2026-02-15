using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediClinic.Models.ModelViews;

namespace MediClinic.Controllers
{
    public class PhysicianController : Controller
    {
        private readonly MediClinicDbContext _context;

        public PhysicianController(MediClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "User");

            if (role != "Physician")
                return RedirectToAction("AccessDenied", "User");

            return View();
        }

        // ==========================
        // Patient Details Page
        // ==========================
        public IActionResult PatientDetails(int patientId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role))
                return RedirectToAction("Login", "User");

            if (role != "Physician")
                return RedirectToAction("AccessDenied", "User");

            var physicianId = HttpContext.Session.GetInt32("RoleReferenceID");
            if (physicianId == null)
                return RedirectToAction("Login", "User");

            // Patient basic
            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == patientId);
            if (patient == null)
                return NotFound();

            // Medical profile (optional)
            var profile = _context.PatientMedicalProfiles
                .FirstOrDefault(x => x.PatientId == patientId);

            // Previous visits (Completed + advice exists)
            var previousVisits = _context.Schedules
                .Include(s => s.Appointment)
                .Include(s => s.PhysicianAdvices)
                .Where(s =>
                    s.Appointment.PatientId == patientId &&
                    s.ScheduleStatus == "Completed" &&
                    s.PhysicianAdvices.Any()
                )
                .OrderByDescending(s => s.ScheduleDate)
                .Take(10)
                .Select(s => new PatientHistoryVM
                {
                    ScheduleId = s.ScheduleId,
                    ScheduleDate = s.ScheduleDate,
                    ScheduleTime = s.ScheduleTime,
                    Reason = s.Appointment.Reason,
                    Criticality = s.Appointment.Criticality,
                    Advice = s.PhysicianAdvices.Select(a => a.Advice).FirstOrDefault(),
                    Note = s.PhysicianAdvices.Select(a => a.Note).FirstOrDefault(),
                    PhysicianAdviceId = s.PhysicianAdvices.Select(a => a.PhysicianAdviceId).FirstOrDefault()
                })
                .ToList();

            PatientDetailsVM vm = new PatientDetailsVM();
            vm.PatientId = patient.PatientId;
            vm.PatientName = patient.PatientName;
            vm.Gender = patient.Gender;
            vm.Phone = patient.Phone;
            vm.Email = patient.Email;

            vm.Allergies = profile?.MedicalAllergies;
            vm.PastIllness = profile?.MedicalPastIllness;
            vm.ChronicDiseases = profile?.MedicalChronicDiseases;
            vm.Notes = profile?.MedicalNotes;

            vm.PreviousVisits = previousVisits;

            return View(vm);
        }
    }
}
