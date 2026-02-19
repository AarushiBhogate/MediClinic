using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MediClinic.Controllers
{
    public class UserController : Controller
    {
        private readonly MediClinicDbContext _context;

        public UserController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ================= REGISTER =================
        // ==============================
        // REGISTER (GET)
        // ==============================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ==============================
        // REGISTER (POST)
        // ==============================
        [HttpPost]
        public IActionResult Register(RegisterVM vm)
        {
            if (vm.Role != "Patient" && vm.Role != "Physician")
            {
                ViewBag.Error = "Only Patient and Physician registration allowed.";
                return View(vm);
            }

            var existingUser = _context.Users
                .FirstOrDefault(x => x.UserName == vm.UserName);

            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists.";
                return View(vm);
            }

            int roleReferenceId = 0;

            if (vm.Role == "Patient")
            {
                Patient p = new Patient
                {
                    PatientName = vm.FullName,
                    Dob = vm.DOB,
                    Gender = vm.Gender,
                    Address = vm.Address,
                    Phone = vm.Phone,
                    Email = vm.Email,
                    Summary = "Registered from website",
                    PatientStatus = "Active"
                };

                _context.Patients.Add(p);
                _context.SaveChanges();

                roleReferenceId = p.PatientId;
            }
            else if (vm.Role == "Physician")
            {
                Physician d = new Physician
                {
                    PhysicianName = vm.FullName,
                    Specialization = vm.Specialization,
                    Address = vm.Address,
                    Phone = vm.Phone,
                    Email = vm.Email,
                    Summary = vm.Summary,
                    PhysicianStatus = "Active"
                };

                _context.Physicians.Add(d);
                _context.SaveChanges();

                roleReferenceId = d.PhysicianId;
            }

            User u = new User
            {
                UserName = vm.UserName,
                Password = vm.Password, // demo only
                Role = vm.Role,
                RoleReferenceId = roleReferenceId,
                Status = "Active"
            };

            _context.Users.Add(u);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // ==============================
        // LOGIN (GET)
        // ==============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ==============================
        // LOGIN (POST)
        // ==============================
        [HttpPost]
        public IActionResult Login(string UserName, string Password)
        {
            // 1️⃣ Check if user exists & is Active
            var result = _context.Users.FirstOrDefault(x =>
                x.UserName == UserName &&
                x.Password == Password &&
                x.Status == "Active");

            if (result == null)
            {
                ViewBag.Error = "Invalid Username or Password";
                return View();
            }

            // Store Session (IMPORTANT: consistent naming)
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetString("UserName", result.UserName);
            HttpContext.Session.SetString("Role", result.Role);

            if (result.RoleReferenceId != null)
                HttpContext.Session.SetInt32("RoleReferenceID", result.RoleReferenceId.Value);

            // Role-based redirect
            if (result.Role == "Patient")
            {
                HttpContext.Session.SetInt32("PatientId", result.RoleReferenceId.Value);
                return RedirectToAction("Dashboard", "Patients");
            }

            if (result.Role == "Physician")
            {
                HttpContext.Session.SetInt32("PhysicianId", result.RoleReferenceId.Value);
                return RedirectToAction("Dashboard", "Physician");
            }

            if (result.Role == "Admin")
                return RedirectToAction("Index", "Admin");


            return RedirectToAction("Index", "Home");
        }

        // ==============================
        // CHANGE PASSWORD (GET)
        // ==============================
        public IActionResult ChangePassword()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login");

            return View();
        }

        // ==============================
        // CHANGE PASSWORD (POST)
        // ==============================
        [HttpPost]
        public IActionResult ChangePassword(string currentPassword,
                                            string newPassword,
                                            string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return RedirectToAction("Login");

            if (user.Password != currentPassword)
            {
                ViewBag.Error = "Current password is incorrect.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New password and confirm password do not match.";
                return View();
            }

            user.Password = newPassword;
            _context.SaveChanges();

            ViewBag.Success = "Password changed successfully.";
            return View();
        }

        // ==============================
        // LOGOUT
        // ==============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
