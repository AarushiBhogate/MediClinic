using MediClinic.Models;
using MediClinic.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;
using System;
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
                a.AppointmentDate.HasValue &&
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);

            if (patient == null) return NotFound();

            return View(patient);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.PatientStatus = "Inactive";
                _context.Update(patient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // ================= BOOK APPOINTMENT =================

        public IActionResult BookAppointment()
        {
            var check = RequireLogin();
            if (check != null) return check;

            return View();
        }

        [HttpPost]
        public IActionResult BookAppointment(Appointment model)
        {
            var check = RequireLogin();
            if (check != null) return check;

            if (!ModelState.IsValid)
                return View(model);

            model.PatientId = PatientId.Value;
            model.ScheduleStatus = "Pending";
            if (model.AppointmentDate.HasValue)
            {
                model.AppointmentDate = model.AppointmentDate.Value.Date;
            }


            _context.Appointments.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Dashboard");
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

        //// ================= EDIT PROFILE =================
        //public IActionResult EditProfile()
        //{
        //    var check = RequireLogin();
        //    if (check != null) return check;

        //    var patient = _context.Patients.Find(PatientId);
        //    return View(patient);
        //}

        //[HttpPost]
        //public IActionResult EditProfile(Patient model)
        //{
        //    var check = RequireLogin();
        //    if (check != null) return check;

        //    var patient = _context.Patients.Find(PatientId);
        //    if (patient == null)
        //        return NotFound();

        //    patient.PatientName = model.PatientName;
        //    patient.Phone = model.Phone;
        //    patient.Address = model.Address;

        //    _context.SaveChanges();
        //    return RedirectToAction("Profile");
        //}

        // ================= MEDICAL PROFILE =================
        

        // GET: Edit Profile
        public IActionResult EditProfile()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientId == PatientId);

            if (patient == null) return NotFound();

            return View(patient);
        }

        // POST: Edit Profile
        [HttpPost]
        public IActionResult EditProfile(Patient model)
        {
            var check = RequireLogin();
            if (check != null) return check;

            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientId == PatientId);

            if (patient == null) return NotFound();

            patient.PatientName = model.PatientName;
            patient.Email = model.Email;
            patient.Phone = model.Phone;
            patient.Address = model.Address;
            patient.Dob = model.Dob;

            _context.SaveChanges();

            return RedirectToAction("Profile"); // ✅ back to profile page
        }


        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile profileImage)
        {
            if (profileImage != null && profileImage.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(),
                                        "wwwroot/images",
                                        fileName);

        // ================= MEDICAL PROFILE =================
        public IActionResult EditMedicalProfile()
        {
            var check = RequireLogin();
            if (check != null) return check;

                var patient = _context.Patients
                    .FirstOrDefault(p => p.PatientId == PatientId);

                if (patient != null)
                {
                    patient.ProfileImage = "/images/" + fileName;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public IActionResult RemoveProfileImage()
        {
            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientId == PatientId);

            if (patient != null && !string.IsNullOrEmpty(patient.ProfileImage))
            {
                var fullPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    patient.ProfileImage.TrimStart('/')
                );

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                patient.ProfileImage = null;
                _context.SaveChanges();
            }

            return RedirectToAction("Profile");
        }






    }
}
