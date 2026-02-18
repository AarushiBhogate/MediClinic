using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediClinic.Models;

public class SupplierController : Controller
{
    private readonly MediClinicDbContext _context;

    public SupplierController(MediClinicDbContext context)
    {
        _context = context;
    }

    // Supplier Dashboard
    public async Task<IActionResult> Dashboard()
    {   
        int supplierId = GetCurrentSupplierId();

        var purchaseOrders = await _context.PurchaseOrderHeaders
            .Where(po => po.SupplierId == supplierId)
            .Include(po => po.PurchaseProductLines)
            .ToListAsync();

        ViewBag.TotalOrders = purchaseOrders.Count;

        ViewBag.Pending = purchaseOrders
            .Count(po => po.PurchaseOrderStatus != null &&
                         po.PurchaseOrderStatus.ToLower() == "pending");

        ViewBag.Accepted = purchaseOrders
            .Count(po => po.PurchaseOrderStatus != null &&
                         po.PurchaseOrderStatus.ToLower() == "accepted");

        ViewBag.Declined = purchaseOrders
            .Count(po => po.PurchaseOrderStatus != null &&
                         po.PurchaseOrderStatus.ToLower() == "declined");

        return View(purchaseOrders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var po = await _context.PurchaseOrderHeaders
            .Include(p => p.PurchaseProductLines)
            .FirstOrDefaultAsync(p => p.Poid == id);

        if (po == null)
            return NotFound();

        return View(po);
    }

    private int GetCurrentSupplierId()
    {
        return 1; // Replace with login session later
    }
}
