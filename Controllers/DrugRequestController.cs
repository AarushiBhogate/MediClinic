using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Controllers
{
    public class DrugRequestController : Controller
    {
        private readonly MediClinicDbContext _context;

        public DrugRequestController(MediClinicDbContext context)
        {
            _context = context;
        }

        private int? GetPhysicianId()
        {
            var role = HttpContext.Session.GetString("Role");
            if (string.IsNullOrEmpty(role)) return null;
            if (role != "Physician") return null;

            return HttpContext.Session.GetInt32("RoleReferenceID");
        }

        // ============================
        // 1) List of My Requests
        // ============================
        public IActionResult Index()
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var requests = _context.DrugRequests
                .Where(r => r.PhysicianId == physicianId)
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            return View(requests);
        }

        // ============================
        // 2) Create Request (GET)
        // ============================
        [HttpGet]
        public IActionResult Create()
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            return View();
        }

        // ============================
        // 3) Create Request (POST)
        // ============================
        [HttpPost]
        public IActionResult Create(string drugsInfoText)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            if (string.IsNullOrWhiteSpace(drugsInfoText))
            {
                ViewBag.Error = "Please enter drug details.";
                return View();
            }

            DrugRequest req = new DrugRequest();
            req.PhysicianId = physicianId.Value;
            req.DrugsInfoText = drugsInfoText;
            req.RequestDate = DateTime.Now;
            req.RequestStatus = "Pending";

            _context.DrugRequests.Add(req);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
