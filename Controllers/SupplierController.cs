using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Mvc;
using MediClinic.Models.ModelViews;

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

            if (supplierId == null)
                return RedirectToAction("Login", "Account");

            var model = new SupplierDashboardVM
            {
                TotalOrders = _context.PurchaseOrderHeaders
                    .Count(p => p.SupplierId == supplierId),

                PendingOrders = _context.PurchaseOrderHeaders
                    .Count(p => p.SupplierId == supplierId && p.PoStatus == "Pending"),

                ApprovedOrders = _context.PurchaseOrderHeaders
                    .Count(p => p.SupplierId == supplierId && p.PoStatus == "Approved"),

                DispatchedOrders = _context.PurchaseOrderHeaders
                    .Count(p => p.SupplierId == supplierId && p.PoStatus == "Dispatched"),

                DeliveredOrders = _context.PurchaseOrderHeaders
                    .Count(p => p.SupplierId == supplierId && p.PoStatus == "Delivered")
            };

            return View(model);
        }

        // ================= VIEW ORDERS =================
        // Keeping your old name
        public IActionResult ViewPendingOrders()
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var orders = _context.PurchaseOrderHeaders
                .Where(p => p.SupplierId == supplierId &&
                            (p.PoStatus == "Pending" || p.PoStatus == "Approved"))
                .OrderByDescending(p => p.Podate)
                .ToList();

            return View(orders);
        }
        // OrderHISTORY
        public IActionResult OrderHistory()
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var orders = _context.PurchaseOrderHeaders
                .Where(p => p.SupplierId == supplierId &&
                           (p.PoStatus == "Delivered" || p.PoStatus == "Rejected"))
                .OrderByDescending(p => p.Podate)
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
        // ================= PROFILE =================
        public IActionResult Profile()
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            if (supplierId == null)
                return RedirectToAction("Login", "User");

            var supplier = _context.Suppliers
                .FirstOrDefault(s => s.SupplierId == supplierId);

            if (supplier == null)
                return RedirectToAction("AccessDenied", "User");

            var model = new SupplierProfileVM
            {
                SupplierId = supplier.SupplierId,
                SupplierName = supplier.SupplierName,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                SupplierStatus = supplier.SupplierStatus
            };

            return View(model);
        }
        // ================= EDIT PROFILE (GET) =================
        [HttpGet]
        public IActionResult EditProfile()
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var supplier = _context.Suppliers
                .FirstOrDefault(s => s.SupplierId == supplierId);

            if (supplier == null)
                return RedirectToAction("AccessDenied", "User");

            var model = new SupplierProfileVM
            {
                SupplierId = supplier.SupplierId,
                SupplierName = supplier.SupplierName,
                Email = supplier.Email,
                Phone = supplier.Phone,
                Address = supplier.Address,
                SupplierStatus = supplier.SupplierStatus
            };

            return View(model);
        }
        // ================= EDIT PROFILE (POST) =================
        [HttpPost]
        public IActionResult EditProfile(SupplierProfileVM vm)
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var supplier = _context.Suppliers
                .FirstOrDefault(s => s.SupplierId == supplierId);

            if (supplier == null)
                return RedirectToAction("AccessDenied", "User");

            // Only allow editing safe fields
            supplier.Email = vm.Email;
            supplier.Phone = vm.Phone;
            supplier.Address = vm.Address;

            _context.SaveChanges();

            return RedirectToAction("Profile");
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordVM vm)
        {
            var auth = RequireSupplier();
            if (auth != null) return auth;

            var supplierId = HttpContext.Session.GetInt32("SupplierId");

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "User");

            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return RedirectToAction("AccessDenied", "User");

            // 1️⃣ Verify old password
            if (user.Password != vm.OldPassword)
            {
                TempData["PasswordError"] = "Old password is incorrect.";
                return RedirectToAction("Profile");
            }

            // 2️⃣ Validate new password
            if (string.IsNullOrEmpty(vm.NewPassword) ||
                vm.NewPassword.Length < 6)
            {
                TempData["PasswordError"] = "New password must be at least 6 characters.";
                return RedirectToAction("Profile");
            }

            // 3️⃣ Confirm password match
            if (vm.NewPassword != vm.ConfirmPassword)
            {
                TempData["PasswordError"] = "Passwords do not match.";
                return RedirectToAction("Profile");
            }

            // 4️⃣ Update password
            user.Password = vm.NewPassword;
            _context.SaveChanges();

            TempData["PasswordSuccess"] = "Password changed successfully.";

            return RedirectToAction("Profile");
        }
    }
}