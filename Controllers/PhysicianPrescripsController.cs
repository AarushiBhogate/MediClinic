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
    public class PhysicianPrescripsController : Controller
    {
        private readonly MediClinicDbContext _context;

        public PhysicianPrescripsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // GET: PhysicianPrescrips
        public async Task<IActionResult> Index()
        {
            var mediClinicDbContext = _context.PhysicianPrescrips.Include(p => p.Drug).Include(p => p.PhysicianAdvice);
            return View(await mediClinicDbContext.ToListAsync());
        }

        // GET: PhysicianPrescrips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicianPrescrip = await _context.PhysicianPrescrips
                .Include(p => p.Drug)
                .Include(p => p.PhysicianAdvice)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);
            if (physicianPrescrip == null)
            {
                return NotFound();
            }

            return View(physicianPrescrip);
        }

        // GET: PhysicianPrescrips/Create
        public IActionResult Create()
        {
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId");
            ViewData["PhysicianAdviceId"] = new SelectList(_context.PhysicianAdvices, "PhysicianAdviceId", "PhysicianAdviceId");
            return View();
        }

        // POST: PhysicianPrescrips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PrescriptionId,PhysicianAdviceId,DrugId,Prescription,Dosage")] PhysicianPrescrip physicianPrescrip)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physicianPrescrip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId", physicianPrescrip.DrugId);
            ViewData["PhysicianAdviceId"] = new SelectList(_context.PhysicianAdvices, "PhysicianAdviceId", "PhysicianAdviceId", physicianPrescrip.PhysicianAdviceId);
            return View(physicianPrescrip);
        }

        // GET: PhysicianPrescrips/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicianPrescrip = await _context.PhysicianPrescrips.FindAsync(id);
            if (physicianPrescrip == null)
            {
                return NotFound();
            }
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId", physicianPrescrip.DrugId);
            ViewData["PhysicianAdviceId"] = new SelectList(_context.PhysicianAdvices, "PhysicianAdviceId", "PhysicianAdviceId", physicianPrescrip.PhysicianAdviceId);
            return View(physicianPrescrip);
        }

        // POST: PhysicianPrescrips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrescriptionId,PhysicianAdviceId,DrugId,Prescription,Dosage")] PhysicianPrescrip physicianPrescrip)
        {
            if (id != physicianPrescrip.PrescriptionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physicianPrescrip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicianPrescripExists(physicianPrescrip.PrescriptionId))
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
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId", physicianPrescrip.DrugId);
            ViewData["PhysicianAdviceId"] = new SelectList(_context.PhysicianAdvices, "PhysicianAdviceId", "PhysicianAdviceId", physicianPrescrip.PhysicianAdviceId);
            return View(physicianPrescrip);
        }

        // GET: PhysicianPrescrips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicianPrescrip = await _context.PhysicianPrescrips
                .Include(p => p.Drug)
                .Include(p => p.PhysicianAdvice)
                .FirstOrDefaultAsync(m => m.PrescriptionId == id);
            if (physicianPrescrip == null)
            {
                return NotFound();
            }

            return View(physicianPrescrip);
        }

        // POST: PhysicianPrescrips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physicianPrescrip = await _context.PhysicianPrescrips.FindAsync(id);
            if (physicianPrescrip != null)
            {
                _context.PhysicianPrescrips.Remove(physicianPrescrip);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhysicianPrescripExists(int id)
        {
            return _context.PhysicianPrescrips.Any(e => e.PrescriptionId == id);
        }
    }
}
