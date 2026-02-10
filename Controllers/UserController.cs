using MediClinic.Models;
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
        public IActionResult Register(User user)
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

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }
    }
}
