using MediClinic.Models;
using MediClinic.Models.ModelViews;
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
        public IActionResult Register(RegisterVM vm)
        {
            user.Status = "Active";
            _context.Users.Add(user);
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
        public IActionResult Login(User user)
        {
            var result = _context.Users.FirstOrDefault(x =>
                x.UserName == user.UserName &&
                x.Password == user.Password &&
                x.Status == "Active");

            if (result != null)
            {
                return RedirectToAction("Index", "Home");
            }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}


