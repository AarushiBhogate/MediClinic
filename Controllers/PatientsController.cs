using MediClinic.Models;
using MediClinic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Controllers
{
    public class PatientsController : PatientBaseController
    {
        private readonly MediClinicDbContext _context;

        public PatientsController(MediClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var patient = _context.Patients.Find(PatientId);

            var nextAppointment = _context.Appointments
                .Where(a => a.PatientId == PatientId)
                .OrderBy(a => a.AppointmentDate)
                .FirstOrDefault();

            var totalAppointments = _context.Appointments
                .Count(a => a.PatientId == PatientId);

            var totalPrescriptions = _context.PhysicianPrescrips
                .Include(p => p.PhysicianAdvice)
                .ThenInclude(pa => pa.Schedule)
                .ThenInclude(s => s.Appointment)
                .Count(p => p.PhysicianAdvice.Schedule.Appointment.PatientId == PatientId);

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

        public IActionResult Profile()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var patient = _context.Patients.Find(PatientId);
            return View(patient);
        }

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

            patient.PatientName = model.PatientName;
            patient.Phone = model.Phone;
            patient.Address = model.Address;

            _context.SaveChanges();
            return RedirectToAction("Profile");
        }
    }
}













