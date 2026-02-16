using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Collections.Generic;

namespace MediClinic.Controllers
{
    [Authorize(Roles = "Admin")]   // 🔐 Only Admin can access
    public class AdminController : Controller
    {
        private readonly MediClinicDbContext _context;

        public AdminController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ================= ADMIN DASHBOARD =================
        public IActionResult Index()
        {
            // 📊 Dashboard Counts
            ViewBag.TotalPhysicians = _context.Physicians.Count();
            ViewBag.TotalPatients = _context.Patients.Count();
            ViewBag.TotalSuppliers = _context.Suppliers.Count();
            ViewBag.ChemistCount = _context.Chemists.Count();
           
            ViewBag.ScheduleCount = _context.Schedules.Count();
            
            
           
            
            
            ViewBag.TotalAppointments = _context.Appointments.Count();
            ViewBag.TotalUsers = _context.Users.Count();

            // 🔥 Pending Count
            ViewBag.PendingPatientCount = _context.Patients
                .Count(p => p.PatientStatus == "Pending");

            ViewBag.UserName = User.Identity.Name;

            return View();
        }


        // ================= PENDING PATIENTS PAGE =================
        public IActionResult PendingPatients()
        {
            var pendingPatients = _context.Patients
                .Where(p => p.PatientStatus == "Pending")
                .ToList();

            ViewBag.PendingPatientCount = pendingPatients.Count;

            return View(pendingPatients);
        }


        // ================= APPROVE PATIENT =================
        public IActionResult ApprovePatient(int id)
        {
            var patient = _context.Patients.Find(id);

            if (patient != null)
            {
                // 1️⃣ Change status
                patient.PatientStatus = "Active";

                // 2️⃣ Create login only if not exists
                string username = patient.Email;
                string password = "Patient@123"; // default password

                var existingUser = _context.Users
                    .FirstOrDefault(u => u.UserName == username);

                if (existingUser == null)
                {
                    User newUser = new User()
                    {
                        UserName = username,
                        Password = password,
                        Role = "Patient",
                        Status = "Active"
                    };

                    _context.Users.Add(newUser);
                }

                _context.SaveChanges();
            }

            return RedirectToAction("PendingPatients");
        }


        // ================= DENY PATIENT =================
        public IActionResult DenyPatient(int id)
        {
            var patient = _context.Patients.Find(id);

            if (patient != null)
            {
                patient.PatientStatus = "Inactive";

                // Optional: Also deactivate login if exists
                var user = _context.Users
                    .FirstOrDefault(u => u.UserName == patient.Email);

                if (user != null)
                {
                    user.Status = "Inactive";
                }

                _context.SaveChanges();
            }

            return RedirectToAction("PendingPatients");
        }


        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            return RedirectToAction("Logout", "User");
        }
    }
}
