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
    public class PurchaseProductLinesController : Controller
    {
        private readonly MediClinicDbContext _context;

        public PurchaseProductLinesController(MediClinicDbContext context)
        {
            _context = context;
        }

        // GET: PurchaseProductLines
        public async Task<IActionResult> Index()
        {
            var mediClinicDbContext = _context.PurchaseProductLines.Include(p => p.Drug).Include(p => p.Po);
            return View(await mediClinicDbContext.ToListAsync());
        }

        // GET: PurchaseProductLines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseProductLine = await _context.PurchaseProductLines
                .Include(p => p.Drug)
                .Include(p => p.Po)
                .FirstOrDefaultAsync(m => m.PolineId == id);
            if (purchaseProductLine == null)
            {
                return NotFound();
            }

            return View(purchaseProductLine);
        }

        // GET: PurchaseProductLines/Create
        public IActionResult Create()
        {
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId");
            ViewData["Poid"] = new SelectList(_context.PurchaseOrderHeaders, "Poid", "Poid");
            return View();
        }

        // POST: PurchaseProductLines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PolineId,Poid,DrugId,SlNo,Qty,Note")] PurchaseProductLine purchaseProductLine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseProductLine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId", purchaseProductLine.DrugId);
            ViewData["Poid"] = new SelectList(_context.PurchaseOrderHeaders, "Poid", "Poid", purchaseProductLine.Poid);
            return View(purchaseProductLine);
        }

        // GET: PurchaseProductLines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseProductLine = await _context.PurchaseProductLines.FindAsync(id);
            if (purchaseProductLine == null)
            {
                return NotFound();
            }
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId", purchaseProductLine.DrugId);
            ViewData["Poid"] = new SelectList(_context.PurchaseOrderHeaders, "Poid", "Poid", purchaseProductLine.Poid);
            return View(purchaseProductLine);
        }

        // POST: PurchaseProductLines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PolineId,Poid,DrugId,SlNo,Qty,Note")] PurchaseProductLine purchaseProductLine)
        {
            if (id != purchaseProductLine.PolineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseProductLine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseProductLineExists(purchaseProductLine.PolineId))
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
            ViewData["DrugId"] = new SelectList(_context.Drugs, "DrugId", "DrugId", purchaseProductLine.DrugId);
            ViewData["Poid"] = new SelectList(_context.PurchaseOrderHeaders, "Poid", "Poid", purchaseProductLine.Poid);
            return View(purchaseProductLine);
        }

        // GET: PurchaseProductLines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseProductLine = await _context.PurchaseProductLines
                .Include(p => p.Drug)
                .Include(p => p.Po)
                .FirstOrDefaultAsync(m => m.PolineId == id);
            if (purchaseProductLine == null)
            {
                return NotFound();
            }

            return View(purchaseProductLine);
        }

        // POST: PurchaseProductLines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseProductLine = await _context.PurchaseProductLines.FindAsync(id);
            if (purchaseProductLine != null)
            {
                _context.PurchaseProductLines.Remove(purchaseProductLine);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PurchaseProductLineExists(int id)
        {
            return _context.PurchaseProductLines.Any(e => e.PolineId == id);
        }
    }
}
