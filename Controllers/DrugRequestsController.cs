using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediClinic.Controllers
{
    public class DrugRequestsController : PatientBaseController
    {
        private readonly MediClinicDbContext _context;

        public DrugRequestsController(MediClinicDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var check = RequireLogin();
            if (check != null) return check;

            var requests = _context.DrugRequests
                .OrderByDescending(r => r.RequestDate)
                .ToList();

            return View(requests);
        }
    }
}





