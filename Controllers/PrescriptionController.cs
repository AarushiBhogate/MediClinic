using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly MediClinicDbContext _context;

        public PrescriptionController(MediClinicDbContext context)
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

        // ==========================
        // 1) Create Prescription (GET)
        // ==========================
        [HttpGet]
        public IActionResult Create(int physicianAdviceId)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var advice = _context.PhysicianAdvices
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Appointment)
                        .ThenInclude(ap => ap.Patient)
                .FirstOrDefault(a => a.PhysicianAdviceId == physicianAdviceId);

            if (advice == null) return NotFound();

            // safety check: logged in physician should own schedule
            if (advice.Schedule?.PhysicianId != physicianId)
                return RedirectToAction("AccessDenied", "User");

            ViewBag.Drugs = _context.Drugs
                .Select(d => new SelectListItem
                {
                    Value = d.DrugId.ToString(),
                    Text = d.DrugTitle
                })
                .ToList();

            ViewBag.Advice = advice;
            return View();
        }

        // ==========================
        // 2) Create Prescription (POST)
        // ==========================
        [HttpPost]
        public IActionResult Create(int physicianAdviceId, int drugId, string dosage, string prescription)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var advice = _context.PhysicianAdvices
                .Include(a => a.Schedule)
                .FirstOrDefault(a => a.PhysicianAdviceId == physicianAdviceId);

            if (advice == null) return NotFound();

            if (advice.Schedule?.PhysicianId != physicianId)
                return RedirectToAction("AccessDenied", "User");

            PhysicianPrescrip pp = new PhysicianPrescrip();
            pp.PhysicianAdviceId = physicianAdviceId;
            pp.DrugId = drugId;
            pp.Dosage = dosage;
            pp.Prescription = prescription;

            _context.PhysicianPrescrips.Add(pp);
            _context.SaveChanges();
            ViewBag.Advice = advice;


            return RedirectToAction("View", new { physicianAdviceId });

        }

        // ==========================
        // 3) View Prescription
        // ==========================
        public IActionResult View(int physicianAdviceId)
        {
            var physicianId = GetPhysicianId();
            if (physicianId == null) return RedirectToAction("Login", "User");

            var advice = _context.PhysicianAdvices
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Appointment)
                        .ThenInclude(ap => ap.Patient)
                .FirstOrDefault(a => a.PhysicianAdviceId == physicianAdviceId);

            if (advice == null) return NotFound();

            if (advice.Schedule?.PhysicianId != physicianId)
                return RedirectToAction("AccessDenied", "User");

            var list = _context.PhysicianPrescrips
                .Include(p => p.Drug)
                .Where(p => p.PhysicianAdviceId == physicianAdviceId)
                .ToList();

            ViewBag.Advice = advice;
            //ViewBag.List = list;
            ViewBag.PhysicianAdviceId = physicianAdviceId;
            ViewBag.Prescriptions = list;
            ViewBag.PatientName = advice?.Schedule?.Appointment?.Patient?.PatientName;
            ViewBag.Date = advice?.Schedule?.ScheduleDate;
            ViewBag.Time = advice?.Schedule?.ScheduleTime;

            return View();
        }
    }
}
