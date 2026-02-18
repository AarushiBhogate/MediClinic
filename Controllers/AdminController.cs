using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MediClinic.Controllers
{
    public class AdminController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AdminController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ================= ADMIN ROLE CHECK =================
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin";
        }

        // ================= DASHBOARD =================
        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            ViewBag.TotalPhysicians = _context.Physicians.Count();
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.TotalSuppliers = _context.Suppliers.Count();
            ViewBag.ChemistCount = _context.Chemists.Count();
            ViewBag.ScheduleCount = _context.Schedules.Count();
            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.TotalUsers = _context.Users.Count();

            ViewBag.PendingPatientCount = _context.Patients
                .Count(p => p.PatientStatus == "Pending");

            return View();
        }

        // ================= PENDING PATIENTS =================
        public IActionResult PendingPatients()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            var pendingPatients = _context.Patients
                .Where(p => p.PatientStatus == "Pending")
                .ToList();

            return View(pendingPatients);
        }

        // ================= APPROVE PATIENT =================
        public IActionResult ApprovePatient(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            var patient = _context.Patients.Find(id);
            if (patient == null) return NotFound();

            patient.PatientStatus = "Active";

            var existingUser = _context.Users
                .FirstOrDefault(u => u.UserName == patient.Email);

            if (existingUser == null)
            {
                _context.Users.Add(new User
                {
                    UserName = patient.Email,
                    Password = "Patient@123",
                    Role = "Patient",
                    Status = "Active"
                });
            }

            _context.SaveChanges();
            return RedirectToAction("PendingPatients");
        }

        // ================= DENY PATIENT =================
        public IActionResult DenyPatient(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            var patient = _context.Patients.Find(id);
            if (patient == null) return NotFound();

            patient.PatientStatus = "Inactive";

            var user = _context.Users
                .FirstOrDefault(u => u.UserName == patient.Email);

            if (user != null)
                user.Status = "Inactive";

            _context.SaveChanges();
            return RedirectToAction("PendingPatients");
        }

        // ================= ADMIN → APPOINTMENTS (Pending Only) =================
        public IActionResult Appointments()
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

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


        // ================= ASSIGN APPOINTMENT (GET) =================
        public IActionResult AssignAppointment(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            var doctors = _context.Physicians
                .Where(p => p.Specialization == appointment.RequiredSpecialization)
                .ToList();

            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientId == appointment.PatientId);

            var vm = new AssignAppointmentVM
            {
                AppointmentId = appointment.AppointmentId,
                PatientName = patient?.PatientName,
                RequestedDate = appointment.AppointmentDate,
                RequiredSpecialization = appointment.RequiredSpecialization,
                AvailableDoctors = doctors,
                ConfirmedDate = DateOnly.FromDateTime(DateTime.Today)
            };

            return View(vm);
        }

        // ================= ASSIGN APPOINTMENT (POST) =================
        [HttpPost]
        public IActionResult AssignAppointment(AssignAppointmentVM vm)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "User");

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.AppointmentId == vm.AppointmentId);

            if (appointment == null)
                return NotFound();

            // Prevent double scheduling
            if (appointment.ScheduleStatus != "Pending")
                return BadRequest("Appointment already scheduled.");

            // Validate doctor specialization
            var doctor = _context.Physicians
                .FirstOrDefault(p => p.PhysicianId == vm.SelectedPhysicianId);

            if (doctor == null ||
                doctor.Specialization != appointment.RequiredSpecialization)
                return BadRequest("Invalid doctor selection.");

            // Check availability
            var alreadyBooked = _context.Schedules.Any(s =>
                s.PhysicianId == vm.SelectedPhysicianId &&
                s.ScheduleDate == vm.ConfirmedDate &&
                s.ScheduleTime == vm.ConfirmedTime &&
                s.ScheduleStatus == "Confirmed");

            if (alreadyBooked)
            {
                ModelState.AddModelError("", "Doctor not available at selected time.");

                vm.AvailableDoctors = _context.Physicians
                    .Where(p => p.Specialization == appointment.RequiredSpecialization)
                    .ToList();

                return View(vm);
            }

            // Create schedule
            var schedule = new Schedule
            {
                AppointmentId = vm.AppointmentId,
                PhysicianId = vm.SelectedPhysicianId,
                ScheduleDate = vm.ConfirmedDate,
                ScheduleTime = vm.ConfirmedTime,
                ScheduleStatus = "Confirmed"
            };

            appointment.ScheduleStatus = "Scheduled";

            _context.Schedules.Add(schedule);
            _context.SaveChanges();

            return RedirectToAction("Appointments");
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "User");
        }
    }
}
