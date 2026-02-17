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

            int? patientId = HttpContext.Session.GetInt32("PatientId");

            var prescriptions = _context.PhysicianPrescrips
                .Include(p => p.PhysicianAdvice)
                .ThenInclude(pa => pa.Schedule)
                .ThenInclude(s => s.Appointment)
                .Where(p => p.PhysicianAdvice.Schedule.Appointment.PatientId == patientId)
                .ToList();

            return View(prescriptions);
        }

        public IActionResult RequestMedicine(int id)
        {
            var request = new DrugRequest
            {
                PhysicianId = 1,
                DrugsInfoText = "Requested from Prescription ID: " + id,
                RequestDate = DateTime.Now,
                RequestStatus = "Pending"
            };

            _context.DrugRequests.Add(request);
            _context.SaveChanges();

            return RedirectToAction("Index", "DrugRequests");
        }
    }
}









