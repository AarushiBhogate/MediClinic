using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Mvc;
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

        // -------- REGISTER (GET) --------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // -------- REGISTER (POST) --------
        [HttpPost]
        public IActionResult Register(RegisterVM vm)
        {
            // 1) Role validation
            if (vm.Role != "Patient" && vm.Role != "Physician")
            {
                ViewBag.Error = "Only Patient and Physician registration is allowed.";
                return View(vm);
            }

            // 2) Username check
            var existingUser = _context.Users.FirstOrDefault(x => x.UserName == vm.UserName);
            if (existingUser != null)
            {
                ViewBag.Error = "Username already exists.";
                return View(vm);
            }

            int roleReferenceId = 0;

            // 3) Insert into Patient / Physician table first
            if (vm.Role == "Patient")
            {
                Patient p = new Patient();
                p.PatientName = vm.FullName;
                p.Dob = vm.DOB;
                p.Gender = vm.Gender;
                p.Address = vm.Address;
                p.Phone = vm.Phone;
                p.Email = vm.Email;
                p.Summary = "Registered from website";
                p.PatientStatus = "Active";

                _context.Patients.Add(p);
                _context.SaveChanges();

                roleReferenceId = p.PatientId;
            }
            else if (vm.Role == "Physician")
            {
                Physician d = new Physician();
                d.PhysicianName = vm.FullName;
                d.Specialization = vm.Specialization;
                d.Address = vm.Address;
                d.Phone = vm.Phone;
                d.Email = vm.Email;
                d.Summary = vm.Summary;
                d.PhysicianStatus = "Active";

                _context.Physicians.Add(d);
                _context.SaveChanges();

                roleReferenceId = d.PhysicianId;
            }

            // 4) Insert into Users table
            User u = new User();
            u.UserName = vm.UserName;
            u.Password = vm.Password; // (later hashing karna)
            u.Role = vm.Role;
            u.RoleReferenceId = roleReferenceId;
            u.Status = "Active";

            _context.Users.Add(u);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }


        // -------- LOGIN (GET) --------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // -------- LOGIN (POST) --------
        [HttpPost]
        //public IActionResult Login(User user)
        //{
        //    var result = _context.Users.FirstOrDefault(x =>
        //        x.UserName == user.UserName &&
        //        x.Password == user.Password &&
        //        x.Status == "Active");

        //    if (result != null)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    ViewBag.Error = "Invalid Username or Password";
        //    return View();
        //}
        [HttpPost]
        public IActionResult Login(User user)
        {
            var result = _context.Users.FirstOrDefault(x =>
                x.UserName == user.UserName &&
                x.Password == user.Password &&
                x.Status == "Active");

            if (result != null)
            {
                // Store session
                HttpContext.Session.SetInt32("UserID", result.UserId);
                HttpContext.Session.SetString("UserName", result.UserName);
                HttpContext.Session.SetString("Role", result.Role);

                if (result.RoleReferenceId != null)
                    HttpContext.Session.SetInt32("RoleReferenceID", result.RoleReferenceId.Value);

                // Redirect based on role
                if (result.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");

                if (result.Role == "Physician")
                    return RedirectToAction("Dashboard", "Physician");

                if (result.Role == "Patient")
                    return RedirectToAction("Dashboard", "Patient");

                if (result.Role == "Supplier")
                    return RedirectToAction("Dashboard", "Supplier");

                if (result.Role == "Chemist")
                    return RedirectToAction("Dashboard", "Chemist");

                // fallback
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

    }
}
