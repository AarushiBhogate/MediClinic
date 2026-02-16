using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediClinic.Controllers
{
    public class PrescriptionsController : PatientBaseController
    {
        private readonly MediClinicDbContext _context;

        public PrescriptionsController(MediClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var list = _context.PhysicianAdvices
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Physician)
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Appointment)
                .Where(a => a.Schedule.Appointment.PatientId == PatientId)
                .OrderByDescending(a => a.PhysicianAdviceId)
                .ToList();

            return View(list);
        }

        public IActionResult Details(int id)
        {
            var check = RequireLogin();
            if (check != null) return check;

            var advice = _context.PhysicianAdvices
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Physician)
                .Include(a => a.Schedule)
                    .ThenInclude(s => s.Appointment)
                .FirstOrDefault(a => a.PhysicianAdviceId == id);

            if (advice == null) return NotFound();

            // security: patient should only see their own advice
            if (advice.Schedule.Appointment.PatientId != PatientId)
                return RedirectToAction("AccessDenied", "User");

            var medicines = _context.PhysicianPrescrips
                .Include(p => p.Drug)
                .Where(p => p.PhysicianAdviceId == id)
                .ToList();

            ViewBag.Medicines = medicines;

            return View(advice);
        }
    }
}
