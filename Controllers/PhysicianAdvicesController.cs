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
    public class PhysicianAdvicesController : Controller
    {
        private readonly MediClinicDbContext _context;

        public PhysicianAdvicesController(MediClinicDbContext context)
        {
            _context = context;
        }

        // GET: PhysicianAdvices
        public async Task<IActionResult> Index()
        {
            var mediClinicDbContext = _context.PhysicianAdvices.Include(p => p.Schedule);
            return View(await mediClinicDbContext.ToListAsync());
        }

        // GET: PhysicianAdvices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicianAdvice = await _context.PhysicianAdvices
                .Include(p => p.Schedule)
                .FirstOrDefaultAsync(m => m.PhysicianAdviceId == id);
            if (physicianAdvice == null)
            {
                return NotFound();
            }

            return View(physicianAdvice);
        }

        // GET: PhysicianAdvices/Create
        public IActionResult Create()
        {
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "ScheduleId", "ScheduleId");
            return View();
        }

        // POST: PhysicianAdvices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhysicianAdviceId,ScheduleId,Advice,Note")] PhysicianAdvice physicianAdvice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physicianAdvice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "ScheduleId", "ScheduleId", physicianAdvice.ScheduleId);
            return View(physicianAdvice);
        }

        // GET: PhysicianAdvices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicianAdvice = await _context.PhysicianAdvices.FindAsync(id);
            if (physicianAdvice == null)
            {
                return NotFound();
            }
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "ScheduleId", "ScheduleId", physicianAdvice.ScheduleId);
            return View(physicianAdvice);
        }

        // POST: PhysicianAdvices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhysicianAdviceId,ScheduleId,Advice,Note")] PhysicianAdvice physicianAdvice)
        {
            if (id != physicianAdvice.PhysicianAdviceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physicianAdvice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicianAdviceExists(physicianAdvice.PhysicianAdviceId))
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
            ViewData["ScheduleId"] = new SelectList(_context.Schedules, "ScheduleId", "ScheduleId", physicianAdvice.ScheduleId);
            return View(physicianAdvice);
        }

        // GET: PhysicianAdvices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicianAdvice = await _context.PhysicianAdvices
                .Include(p => p.Schedule)
                .FirstOrDefaultAsync(m => m.PhysicianAdviceId == id);
            if (physicianAdvice == null)
            {
                return NotFound();
            }

            return View(physicianAdvice);
        }

        // POST: PhysicianAdvices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physicianAdvice = await _context.PhysicianAdvices.FindAsync(id);
            if (physicianAdvice != null)
            {
                _context.PhysicianAdvices.Remove(physicianAdvice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhysicianAdviceExists(int id)
        {
            return _context.PhysicianAdvices.Any(e => e.PhysicianAdviceId == id);
        }
    }
}
