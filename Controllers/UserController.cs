using MediClinic.Models;
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
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Status = "Active"; // Default active for manual users (Admin etc.)

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(user);
        }

        // ================= LOGIN =================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            // 1️⃣ Check if user exists & is Active
            var result = _context.Users.FirstOrDefault(x =>
                x.UserName == user.UserName &&
                x.Password == user.Password &&
                x.Status == "Active");

            if (result == null)
            {
                ViewBag.Error = "Invalid Username or Password";
                return View();
            }

            // 2️⃣ Extra check for Patient role
            if (result.Role == "Patient")
            {
                var patient = _context.Patients
                    .FirstOrDefault(p => p.Email == result.UserName);

                if (patient == null || patient.PatientStatus != "Active")
                {
                    ViewBag.Error = "Your account is not approved by Admin yet.";
                    return View();
                }
            }

            // ================= COOKIE AUTH =================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.UserName),
                new Claim(ClaimTypes.Role, result.Role ?? "User")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("UserName", result.UserName);
            HttpContext.Session.SetString("UserRole", result.Role);

            // ================= ROLE REDIRECTION =================
            switch (result.Role)
            {
                case "Admin":
                    return RedirectToAction("Index", "Admin");

                case "Physician":
                    return RedirectToAction("Index", "Physician");

                case "Patient":
                    return RedirectToAction("Index", "Patient");

                case "Supplier":
                    return RedirectToAction("Index", "Supplier");

                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        // ================= DASHBOARD =================
        [Authorize]
        public IActionResult Dashboard()
        {
            ViewBag.UserName = HttpContext.User.Identity.Name;
            ViewBag.UserRole = HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return View();
        }

        // ================= LOGOUT =================
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}