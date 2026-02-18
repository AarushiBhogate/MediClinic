using MediClinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MediClinic.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class PhysiciansController : Controller
    {
        private readonly MediClinicDbContext _context;


        public PhysiciansController(MediClinicDbContext context)
        {
            _context = context;
        }

        // GET: Physicians
        public async Task<IActionResult> Index()
        {
            return View(await _context.Physicians.ToListAsync());
        }

        // GET: Physicians/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians
                .FirstOrDefaultAsync(m => m.PhysicianId == id);
            if (physician == null)
            {
                return NotFound();
            }

            return View(physician);
        }

        // GET: Physicians/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Physicians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physician);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(physician);
        }

        // GET: Physicians/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians.FindAsync(id);
            if (physician == null)
            {
                return NotFound();
            }
            return View(physician);
        }

        // POST: Physicians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhysicianId,PhysicianName,Specialization,Address,Phone,Email,Summary,PhysicianStatus")] Physician physician)
        {
            if (id != physician.PhysicianId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physician);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicianExists(physician.PhysicianId))
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
            return View(physician);
        }

        // GET: Physicians/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physician = await _context.Physicians
                .FirstOrDefaultAsync(m => m.PhysicianId == id);
            if (physician == null)
            {
                return NotFound();
            }

            return View(physician);
        }

        // POST: Physicians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physician = await _context.Physicians.FindAsync(id);
            if (physician != null)
            {
                _context.Physicians.Remove(physician);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhysicianExists(int id)
        {
            return _context.Physicians.Any(e => e.PhysicianId == id);
        }
    }
}
