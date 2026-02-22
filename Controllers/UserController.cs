using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace MediClinic.Controllers
{
    public class UserController : Controller
    {
        private readonly MediClinicDbContext _context;

        public UserController(MediClinicDbContext context)
        {
            _context = context;
        }

        // ================= REGISTER (GET) =================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ================= REGISTER (POST) =================
        [HttpPost]
        public IActionResult Register(RegisterVM vm)
        {
            // Allow all roles
            if (vm.Role != "Patient" &&
                vm.Role != "Physician" &&
                vm.Role != "Chemist" &&
                vm.Role != "Supplier")
            {
                ViewBag.Error = "Invalid role selected.";
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

            // ===== PATIENT =====
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
                    PatientStatus = "Pending"
                };

                _context.Patients.Add(p);
                _context.SaveChanges();

                roleReferenceId = p.PatientId;
            }

            // ===== PHYSICIAN =====
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

            // ===== CHEMIST =====
            else if (vm.Role == "Chemist")
            {
                Chemist c = new Chemist
                {
                    ChemistName = vm.FullName,
                    Address = vm.Address,
                    Phone = vm.Phone,
                    Email = vm.Email,
                    Summary = "Registered from website",
                    ChemistStatus = "Active"
                };

                _context.Chemists.Add(c);
                _context.SaveChanges();

                roleReferenceId = c.ChemistId;
            }

            // ===== SUPPLIER =====
            else if (vm.Role == "Supplier")
            {
                Supplier s = new Supplier
                {
                    SupplierName = vm.FullName,
                    Address = vm.Address,
                    Phone = vm.Phone,
                    Email = vm.Email,
                    SupplierStatus = "Active"
                };

                _context.Suppliers.Add(s);
                _context.SaveChanges();

                roleReferenceId = s.SupplierId;
            }

            // ===== CREATE USER =====
            User u = new User
            {
                UserName = vm.UserName,
                Password = vm.Password, // ⚠️ Plain text (not secure)
                Role = vm.Role,
                RoleReferenceId = roleReferenceId,
                Status = "Active"
            };

            _context.Users.Add(u);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // ================= LOGIN (GET) =================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ================= LOGIN (POST) =================
        [HttpPost]
        public IActionResult Login(string UserName, string Password)
        {
            var result = _context.Users.FirstOrDefault(x =>
                x.UserName == UserName &&
                x.Password == Password &&
                x.Status == "Active");

            if (result == null)
            {
                ViewBag.Error = "Invalid Username or Password";
                return View();
            }

            // Store session
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetString("UserName", result.UserName);
            HttpContext.Session.SetString("Role", result.Role);

            if (result.RoleReferenceId != null)
                HttpContext.Session.SetInt32("RoleReferenceID", result.RoleReferenceId.Value);

            // ===== ROLE BASED REDIRECT =====

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

            if (result.Role == "Chemist")
            {
                HttpContext.Session.SetInt32("ChemistId", result.RoleReferenceId.Value);
                return RedirectToAction("Dashboard", "Chemist");
            }

            if (result.Role == "Supplier")
            {
                HttpContext.Session.SetInt32("SupplierId", result.RoleReferenceId.Value);
                return RedirectToAction("Dashboard", "Supplier");
            }

            if (result.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ================= LOGOUT =================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}