using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MediClinic.Models;

namespace MediClinic.Controllers
{
    public class DrugRequestsController : Controller
    {
        private readonly MediClinicDbContext _context;

        public DrugRequestsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // GET: DrugRequests
        public async Task<IActionResult> Index()
        {
            var mediClinicDbContext = _context.DrugRequests.Include(d => d.Physician);
            return View(await mediClinicDbContext.ToListAsync());
        }

        // GET: DrugRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugRequest = await _context.DrugRequests
                .Include(d => d.Physician)
                .FirstOrDefaultAsync(m => m.DrugRequestId == id);
            if (drugRequest == null)
            {
                return NotFound();
            }

            return View(drugRequest);
        }

        // GET: DrugRequests/Create
        public IActionResult Create()
        {
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId");
            return View();
        }

        // POST: DrugRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DrugRequestId,PhysicianId,DrugsInfoText,RequestDate,RequestStatus")] DrugRequest drugRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drugRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId", drugRequest.PhysicianId);
            return View(drugRequest);
        }

        // GET: DrugRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugRequest = await _context.DrugRequests.FindAsync(id);
            if (drugRequest == null)
            {
                return NotFound();
            }
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId", drugRequest.PhysicianId);
            return View(drugRequest);
        }

        // POST: DrugRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DrugRequestId,PhysicianId,DrugsInfoText,RequestDate,RequestStatus")] DrugRequest drugRequest)
        {
            if (id != drugRequest.DrugRequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drugRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrugRequestExists(drugRequest.DrugRequestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PhysicianId"] = new SelectList(_context.Physicians, "PhysicianId", "PhysicianId", drugRequest.PhysicianId);
            return View(drugRequest);
        }

        // GET: DrugRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugRequest = await _context.DrugRequests
                .Include(d => d.Physician)
                .FirstOrDefaultAsync(m => m.DrugRequestId == id);
            if (drugRequest == null)
            {
                return NotFound();
            }

            return View(drugRequest);
        }

        // POST: DrugRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drugRequest = await _context.DrugRequests.FindAsync(id);
            if (drugRequest != null)
            {
                _context.DrugRequests.Remove(drugRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrugRequestExists(int id)
        {
            return _context.DrugRequests.Any(e => e.DrugRequestId == id);
        }
    }
}
