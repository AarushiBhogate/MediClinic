using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace MediClinic.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AdminController(MediClinicDbContext context)
        {
            _context = context;
        }
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }

        // ====================== DASHBOARD ======================
        public IActionResult Index()
        {
            ViewBag.TotalPhysicians = _context.Physicians.Count();
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.TotalSuppliers = _context.Suppliers.Count();
            ViewBag.ChemistCount = _context.Chemists.Count();
            ViewBag.ScheduleCount = _context.Schedules.Count();
            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.TotalUsers = _context.Users.Count();
            ViewBag.PendingPatientCount = _context.Patients.Count(p => p.PatientStatus == "Pending");
            ViewBag.UserName = User.Identity?.Name;

            return View();
        }

        // ====================== PATIENTS ======================
        public async Task<IActionResult> GetPatients()
        {
            var patients = await _context.Patients
                .Where(p => p.PatientStatus != "Pending")
                .ToListAsync();

            return View(patients);
        }

        public async Task<IActionResult> GetPatientById(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null) return NotFound();

            return View(patient);
        }

        public IActionResult CreatPatients()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatients(Patient patient)
        {
            if (!ModelState.IsValid) return View(patient);

            _context.Add(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditPatients(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null) return NotFound();

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatients(int id, Patient patient)
        {
            if (id != patient.PatientId) return NotFound();
            if (!ModelState.IsValid) return View(patient);

            _context.Update(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletePatients(int? id)
        {
            if (id == null) return NotFound();

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null) return NotFound();

            return View(patient);
        }

        [HttpPost, ActionName("DeletePatients")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatientsConfirmed(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                patient.PatientStatus = "Inactive";
                _context.Update(patient);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == patient.Email);

                if (user != null)
                    user.Status = "Inactive";

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        public IActionResult AssignAppointment(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientId == appointment.PatientId);

            var doctors = _context.Physicians
                .Where(p => p.Specialization == appointment.RequiredSpecialization)
                .ToList();

            if (appointment.AppointmentDate.HasValue)
            {
                var requestedDateTime = appointment.AppointmentDate.Value;
                var requestedDate = DateOnly.FromDateTime(requestedDateTime);

                doctors = doctors.Where(doc =>
                {
                    var schedules = _context.Schedules
                        .Where(s => s.PhysicianId == doc.PhysicianId &&
                                    s.ScheduleDate == requestedDate &&
                                    s.ScheduleStatus == "Confirmed")
                        .ToList();

                    return !schedules.Any(s =>
                    {
                        var existingDateTime = DateTime.Parse(
                            s.ScheduleDate.ToString() + " " + s.ScheduleTime);

                        return Math.Abs((existingDateTime - requestedDateTime).TotalMinutes) < 60;
                    });

                }).ToList();
            }

            var vm = new AssignAppointmentVM
            {
                AppointmentId = appointment.AppointmentId,
                PatientName = patient?.PatientName,
                RequestedDate = appointment.AppointmentDate,
                RequiredSpecialization = appointment.RequiredSpecialization,
                AvailableDoctors = doctors,
                ConfirmedDate = appointment.AppointmentDate.HasValue
                    ? DateOnly.FromDateTime(appointment.AppointmentDate.Value)
                    : DateOnly.FromDateTime(DateTime.Today),
                ConfirmedTime = appointment.AppointmentDate.HasValue
                    ? appointment.AppointmentDate.Value.ToString("HH:mm")
                    : "09:00"
            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignAppointment(AssignAppointmentVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == vm.AppointmentId);

            if (appointment == null)
                return NotFound();

            if (appointment.ScheduleStatus != "Pending")
                return BadRequest("Appointment already scheduled.");

            var doctor = _context.Physicians
                .FirstOrDefault(p => p.PhysicianId == vm.SelectedPhysicianId);

            if (doctor == null || doctor.Specialization != appointment.RequiredSpecialization)
                return BadRequest("Invalid doctor selection.");

            // 🔥 Validate Working Hours (9 AM – 5 PM)
            var selectedTime = TimeSpan.Parse(vm.ConfirmedTime);
            var workStart = new TimeSpan(9, 0, 0);
            var workEnd = new TimeSpan(17, 0, 0);

            if (selectedTime < workStart || selectedTime >= workEnd)
            {
                ModelState.AddModelError("", "Doctor works between 9 AM and 5 PM only.");
                vm.AvailableDoctors = _context.Physicians
                    .Where(p => p.Specialization == appointment.RequiredSpecialization)
                    .ToList();
                return View(vm);
            }

            // 🔥 Only 30-minute intervals allowed
            if (selectedTime.Minutes % 30 != 0)
            {
                ModelState.AddModelError("", "Appointments must be in 30-minute intervals.");
                vm.AvailableDoctors = _context.Physicians
                    .Where(p => p.Specialization == appointment.RequiredSpecialization)
                    .ToList();
                return View(vm);
            }

            var confirmedDateTime = DateTime.Parse(
                vm.ConfirmedDate.ToString() + " " + vm.ConfirmedTime);

            // 🔥 Check 1-hour gap conflict
            var doctorSchedules = _context.Schedules
                .Where(s => s.PhysicianId == vm.SelectedPhysicianId &&
                            s.ScheduleDate == vm.ConfirmedDate &&
                            s.ScheduleStatus == "Confirmed")
                .ToList();

            bool hasConflict = doctorSchedules.Any(s =>
            {
                var existingDateTime = DateTime.Parse(
                    s.ScheduleDate.ToString() + " " + s.ScheduleTime);

                return Math.Abs((existingDateTime - confirmedDateTime).TotalMinutes) < 60;
            });

            if (hasConflict)
            {
                var nextAvailable = doctorSchedules
                    .Select(s => DateTime.Parse(
                        s.ScheduleDate.ToString() + " " + s.ScheduleTime))
                    .OrderBy(t => t)
                    .Last()
                    .AddMinutes(60);

                ModelState.AddModelError("",
                    $"Doctor not available. Next available slot: {nextAvailable:hh:mm tt}");

                vm.AvailableDoctors = _context.Physicians
                    .Where(p => p.Specialization == appointment.RequiredSpecialization)
                    .ToList();

                return View(vm);
            }

            // 🔥 Reschedule note
            if (appointment.AppointmentDate.HasValue &&
                appointment.AppointmentDate.Value != confirmedDateTime)
            {
                appointment.AdminNote =
                    "Requested time unavailable. Rescheduled to " +
                    confirmedDateTime.ToString("dd MMM yyyy hh:mm tt");
            }

            appointment.AppointmentDate = confirmedDateTime;
            appointment.ScheduleStatus = "Scheduled";

            var schedule = new Schedule
            {
                AppointmentId = vm.AppointmentId,
                PhysicianId = vm.SelectedPhysicianId,
                ScheduleDate = vm.ConfirmedDate,
                ScheduleTime = vm.ConfirmedTime,
                ScheduleStatus = "Confirmed"
            };

            _context.Schedules.Add(schedule);
            _context.SaveChanges();

            return RedirectToAction("Appointments");
        }


        // ================= PENDING PATIENTS =================
        public IActionResult PendingPatients()
        {
            var pendingPatients = _context.Patients
                .Where(p => p.PatientStatus == "Pending")
                .ToList();
            ViewBag.PendingPatientCount = pendingPatients.Count;
            return View(pendingPatients);
        }

        public IActionResult ApprovePatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                patient.PatientStatus = "Active";
                string username = patient.Email;
                string password = "Patient@123";

                var existingUser = _context.Users.FirstOrDefault(u => u.UserName == username);
                if (existingUser == null)
                {
                    _context.Users.Add(new User
                    {
                        UserName = username,
                        Password = password,
                        Role = "Patient",
                        Status = "Active"
                    });
                }

                _context.SaveChanges();
            }
            return RedirectToAction(nameof(PendingPatients));
        }

        public IActionResult DenyPatient(int id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                patient.PatientStatus = "Inactive";
                var user = _context.Users.FirstOrDefault(u => u.UserName == patient.Email);
                if (user != null) user.Status = "Inactive";
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(PendingPatients));
        }
        // ====================== APPOINTMENTS ======================
        public IActionResult Appointments()
        {
            var list = (from a in _context.Appointments
                        join p in _context.Patients
                        on a.PatientId equals p.PatientId
                        where a.ScheduleStatus == "Pending"
                        orderby a.AppointmentDate descending
                        select new PendingAppointmentVM
                        {
                            AppointmentId = a.AppointmentId,
                            PatientName = p.PatientName,
                            AppointmentDate = a.AppointmentDate,
                            RequiredSpecialization = a.RequiredSpecialization,
                            Criticality = a.Criticality
                        }).ToList();

            return View(list);
        }


        // ====================== LOGOUT ======================
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "User");
        }
    }
}
