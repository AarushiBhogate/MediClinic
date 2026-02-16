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
    public class ChemistsController : Controller
    {
        private readonly MediClinicDbContext _context;

        public ChemistsController(MediClinicDbContext context)
        {
            _context = context;
        }

        // GET: Chemists
        public async Task<IActionResult> Index()
        {
            return View(await _context.Chemists.ToListAsync());
        }

        // GET: Chemists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists
                .FirstOrDefaultAsync(m => m.ChemistId == id);
            if (chemist == null)
            {
                return NotFound();
            }

            return View(chemist);
        }

        // GET: Chemists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chemists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChemistId,ChemistName,Address,Phone,Email,Summary,ChemistStatus")] Chemist chemist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chemist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chemist);
        }

        // GET: Chemists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists.FindAsync(id);
            if (chemist == null)
            {
                return NotFound();
            }
            return View(chemist);
        }

        // POST: Chemists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChemistId,ChemistName,Address,Phone,Email,Summary,ChemistStatus")] Chemist chemist)
        {
            if (id != chemist.ChemistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chemist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChemistExists(chemist.ChemistId))
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
            return View(chemist);
        }

        // GET: Chemists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemist = await _context.Chemists
                .FirstOrDefaultAsync(m => m.ChemistId == id);
            if (chemist == null)
            {
                return NotFound();
            }

            return View(chemist);
        }

        // POST: Chemists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemist = await _context.Chemists.FindAsync(id);
            if (chemist != null)
            {
                _context.Chemists.Remove(chemist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChemistExists(int id)
        {
            return _context.Chemists.Any(e => e.ChemistId == id);
        }
    }
}
