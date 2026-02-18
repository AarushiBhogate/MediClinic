using MediClinic.Models;
using MediClinic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MediClinic.Controllers
{
    public class PatientsController : PatientBaseController
    {
        private readonly MediClinicDbContext _context;

        public PatientsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ================= ADMIN PATIENT LIST =================
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patients.ToListAsync());
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            var check = RequireLogin();
            if (check != null) return check;

            if (PatientId == null)
                return RedirectToAction("Login", "User");

            var patientId = PatientId.Value;

            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientId == patientId);

            var nextAppointment = _context.Appointments
                .Where(a => a.PatientId == patientId &&
                            a.ScheduleStatus != "Cancelled" &&
                            a.AppointmentDate >= DateTime.Now)
                .OrderBy(a => a.AppointmentDate)
                .FirstOrDefault();

            var totalAppointments = _context.Appointments
                .Count(a => a.PatientId == patientId);

            var totalPrescriptions = _context.PhysicianPrescrips
                .Include(p => p.PhysicianAdvice)
                .ThenInclude(pa => pa.Schedule)
                .ThenInclude(s => s.Appointment)
                .Count(p => p.PhysicianAdvice.Schedule.Appointment.PatientId == patientId);

            var pendingRequests = _context.DrugRequests
                .Where(r => r.RequestStatus == "Pending")
                .Count();

            var vm = new PatientDashboardViewModel
            {
                Patient = patient,
                NextAppointment = nextAppointment,
                TotalPrescriptions = totalPrescriptions,
                PendingDrugRequests = pendingRequests
            };

            ViewBag.TotalAppointments = totalAppointments;

            return View(vm);
        }

        // ================= PROFILE =================
        public IActionResult Profile()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var patient = _context.Patients
                .Include(p => p.PatientMedicalProfile)
                .FirstOrDefault(p => p.PatientId == PatientId);

            return View(patient);
        }

        // ================= EDIT PROFILE =================
        public IActionResult EditProfile()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var patient = _context.Patients.Find(PatientId);
            return View(patient);
        }

        [HttpPost]
        public IActionResult EditProfile(Patient model)
        {
            var check = RequireLogin();
            if (check != null) return check;

            var patient = _context.Patients.Find(PatientId);
            if (patient == null)
                return NotFound();

            patient.PatientName = model.PatientName;
            patient.Phone = model.Phone;
            patient.Address = model.Address;

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }

        // ================= MEDICAL PROFILE =================
        public IActionResult EditMedicalProfile()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var profile = _context.PatientMedicalProfiles
                .FirstOrDefault(p => p.PatientId == PatientId);

            if (profile == null)
            {
                profile = new PatientMedicalProfile
                {
                    PatientId = PatientId.Value
                };
            }

            return View(profile);
        }

        [HttpPost]
        public IActionResult EditMedicalProfile(PatientMedicalProfile model)
        {
            var check = RequireLogin();
            if (check != null) return check;

            var profile = _context.PatientMedicalProfiles
                .FirstOrDefault(p => p.PatientId == PatientId);

            if (profile == null)
            {
                model.PatientId = PatientId.Value;
                _context.PatientMedicalProfiles.Add(model);
            }
            else
            {
                profile.MedicalAllergies = model.MedicalAllergies;
                profile.MedicalChronicDiseases = model.MedicalChronicDiseases;
                profile.MedicalPastIllness = model.MedicalPastIllness;
                profile.MedicalNotes = model.MedicalNotes;
            }

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
