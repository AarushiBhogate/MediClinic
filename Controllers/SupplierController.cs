using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediClinic.Controllers
{
    public class SupplierController : Controller
    {
        private readonly MediClinicDbContext _context;

        public SupplierController(MediClinicDbContext context)
        {
            _context = context;
        }

        private bool IsSupplier()
        {
            return HttpContext.Session.GetString("Role") == "Supplier";
        }

        private IActionResult RequireSupplier()
        {
            if (!IsSupplier())
                return RedirectToAction("AccessDenied", "User");

            return null;
        }

        // ================= DASHBOARD =================
        public IActionResult Dashboard()
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            ViewBag.TotalOrders = _context.PurchaseOrderHeaders
                .Count(p => p.SupplierId == supplierId);

            ViewBag.PendingOrders = _context.PurchaseOrderHeaders
                .Count(p => p.SupplierId == supplierId && p.PoStatus == "Pending");

            ViewBag.ApprovedOrders = _context.PurchaseOrderHeaders
                .Count(p => p.SupplierId == supplierId && p.PoStatus == "Approved");

            ViewBag.DispatchedOrders = _context.PurchaseOrderHeaders
                .Count(p => p.SupplierId == supplierId && p.PoStatus == "Dispatched");

            ViewBag.DeliveredOrders = _context.PurchaseOrderHeaders
                .Count(p => p.SupplierId == supplierId && p.PoStatus == "Delivered");

            return View();
        }

        // ================= VIEW ORDERS =================
        // Keeping your old name
        public IActionResult ViewPendingOrders()
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var orders = _context.PurchaseOrderHeaders
                .Where(p => p.SupplierId == supplierId)
                .ToList();

            return View(orders);
        }

        // ================= APPROVE =================
        public IActionResult ApproveOrder(int id)
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var po = _context.PurchaseOrderHeaders
                .FirstOrDefault(p => p.Poid == id && p.SupplierId == supplierId);

            if (po != null && po.PoStatus == "Pending")
            {
                po.PoStatus = "Approved";
                _context.SaveChanges();
            }

            return RedirectToAction("ViewPendingOrders");
        }

        // ================= REJECT =================
        public IActionResult RejectOrder(int id)
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var po = _context.PurchaseOrderHeaders
                .FirstOrDefault(p => p.Poid == id && p.SupplierId == supplierId);

            if (po != null && po.PoStatus == "Pending")
            {
                po.PoStatus = "Rejected";
                _context.SaveChanges();
            }

            return RedirectToAction("ViewPendingOrders");
        }

        // ================= DISPATCH =================
        // DISPATCH
        public IActionResult DispatchOrder(int id)
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var po = _context.PurchaseOrderHeaders
                .FirstOrDefault(p => p.Poid == id && p.SupplierId == supplierId);

            if (po != null && po.PoStatus == "Approved")
            {
                po.PoStatus = "Dispatched";
                _context.SaveChanges();
            }

            return RedirectToAction("ViewPendingOrders");
        }

        // ================= MARK DELIVERED =================
        public IActionResult MarkDelivered(int id)
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var po = _context.PurchaseOrderHeaders
                .FirstOrDefault(p => p.Poid == id && p.SupplierId == supplierId);

            if (po != null && po.PoStatus == "Dispatched")
            {
                po.PoStatus = "Delivered";

                // 🔥 Increase stock ONLY when Delivered
                var lines = _context.PurchaseProductLines
                    .Where(l => l.Poid == po.Poid)
                    .ToList();

                foreach (var line in lines)
                {
                    var drug = _context.Drugs
                        .FirstOrDefault(d => d.DrugId == line.DrugId);

                    if (drug != null)
                        drug.StockQuantity += line.Qty ?? 0;
                }

                _context.SaveChanges();
            }

            return RedirectToAction("ViewPendingOrders");
        }
    }
}