using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: DrugRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var drugRequest = await _context.DrugRequests
                .Include(d => d.Physician)
                .FirstOrDefaultAsync(m => m.DrugRequestId == id);

            if (drugRequest == null)
                return NotFound();

            return View(drugRequest);
        }

        // POST: DrugRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drugRequest = await _context.DrugRequests.FindAsync(id);

            if (drugRequest != null)
                _context.DrugRequests.Remove(drugRequest);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
