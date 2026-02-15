using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediClinic.Controllers
{
    public class UserController : Controller
    {
        private readonly MediClinicDbContext _context;

        public UserController(MediClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserName == username && u.Password == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid login";
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Role", user.Role);

            if (user.Role == "Patient")
            {
                if (user.RoleReferenceId != null)
                    HttpContext.Session.SetInt32("PatientId", user.RoleReferenceId.Value);

                return Redirect("/Patients/Dashboard");
            }

            return Redirect("/Home/Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}


