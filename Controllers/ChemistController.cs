using MediClinic.Models;
using MediClinic.Models.ModelViews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediClinic.Controllers
{
    public class ChemistController : Controller
    {
        private readonly MediClinicDbContext _context;

        public ChemistController(MediClinicDbContext context)
        {
            _context = context;
        }

        private bool IsChemist()
        {
            return HttpContext.Session.GetString("Role") == "Chemist";
        }

        private IActionResult RequireChemist()
        {
            if (!IsChemist())
                return RedirectToAction("AccessDenied", "User");

            return null;
        }
        public IActionResult Dashboard()
        {
            var auth = RequireChemist();
            if (auth != null) return auth;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var nextWeek = today.AddDays(7);

            var vm = new ChemistDashboardVM
            {
                PendingDrugRequests = _context.DrugRequests
                    .Count(d => d.RequestStatus == "Pending"),

                OrdersInProgress = _context.PurchaseOrderHeaders
                    .Count(p => p.PoStatus == "Dispatched"),

                CompletedOrders = _context.PurchaseOrderHeaders
                    .Count(p => p.PoStatus == "Delivered"),

                LowStockItems = _context.Drugs
                    .Count(d =>
                        d.StockQuantity < 5 ||
                        (d.Expiry != null && d.Expiry <= nextWeek)
                    )
            };

            return View(vm);
        }
        public IActionResult ViewDrugRequests(string search, int page = 1)
        {
            var auth = RequireChemist();
            if (auth != null) return auth;

            int pageSize = 5;

            var query = _context.DrugRequests
                .Where(d => d.RequestStatus == "Pending")
                .AsQueryable();

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    d.Physician.PhysicianName.Contains(search) ||
                    d.DrugsInfoText.Contains(search));
            }

            int totalRecords = query.Count();

            var requests = query
                .OrderByDescending(d => d.RequestDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DrugRequestListVM
                {
                    DrugRequestId = d.DrugRequestId,
                    PhysicianName = d.Physician.PhysicianName,
                    DrugsInfoText = d.DrugsInfoText,
                    RequestDate = d.RequestDate,
                    RequestStatus = d.RequestStatus
                })
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.Search = search;

            return View(requests);
        }
        public IActionResult CreatePurchaseOrder(int drugRequestId)
        {

            var auth = RequireChemist();
            if (auth != null) return auth;

            var existingPO = _context.PurchaseOrderHeaders
                .FirstOrDefault(p => p.DrugRequestId == drugRequestId);

            if (existingPO != null)
                return BadRequest("PO already created for this request.");

            var vm = new PurchaseOrderVM
            {
                DrugRequestId = drugRequestId,
                Podate = DateTime.Now,   // ✅ correct
                Pono = GeneratePONumber(), // ✅ correct
                Suppliers = _context.Suppliers
        .Select(s => new SelectListItem
        {
            Value = s.SupplierId.ToString(),
            Text = s.SupplierName
        }).ToList(),
                Drugs = _context.Drugs
    .Where(d => d.DrugStatus == "Active")
    .Select(d => new SelectListItem
    {
        Value = d.DrugId.ToString(),
        Text = d.DrugTitle
    }).ToList(),
                ProductLines = new List<PurchaseProductLineVM>()
            };

            return View(vm);
        }
        private string GeneratePONumber()
        {
            var today = DateTime.Now.ToString("yyyyMMdd");
            var count = _context.PurchaseOrderHeaders.Count() + 1;
            return $"PO-{today}-{count:D3}";
        }
        [HttpPost]
        public IActionResult SavePurchaseOrder(PurchaseOrderVM model)
        {
            var auth = RequireChemist();
            if (auth != null) return auth;

            // ❗ Validation
            if (model.ProductLines == null || !model.ProductLines.Any())
                ModelState.AddModelError("", "Cannot create PO without product lines.");

            if (model.SupplierId == 0)
                ModelState.AddModelError("", "Please select a supplier.");

            var existingPO = _context.PurchaseOrderHeaders
                .FirstOrDefault(p => p.DrugRequestId == model.DrugRequestId);

            if (existingPO != null)
                ModelState.AddModelError("", "PO already created for this request.");

            if (!ModelState.IsValid)
            {
                model.Suppliers = _context.Suppliers
                    .Select(s => new SelectListItem
                    {
                        Value = s.SupplierId.ToString(),
                        Text = s.SupplierName
                    }).ToList();

                model.Drugs = _context.Drugs
                    .Select(d => new SelectListItem
                    {
                        Value = d.DrugId.ToString(),
                        Text = d.DrugTitle
                    }).ToList();

                return View("CreatePurchaseOrder", model);
            }

            using var transaction = _context.Database.BeginTransaction();

            try
            {
                // ✅ Create Header
                var header = new PurchaseOrderHeader
                {
                    Pono = model.Pono,
                    Podate = model.Podate,
                    SupplierId = model.SupplierId,
                    DrugRequestId = model.DrugRequestId == 0 ? null : model.DrugRequestId,
                    PoStatus = "Pending"
                };
                _context.PurchaseOrderHeaders.Add(header);
                _context.SaveChanges(); // Important for header.Poid

                int slno = 1;

                foreach (var line in model.ProductLines)
                {
                    var productLine = new PurchaseProductLine
                    {
                        Poid = header.Poid,
                        DrugId = line.DrugId,
                        Qty = line.Qty,
                        Note = line.Note,
                        SlNo = slno++
                    };

                    _context.PurchaseProductLines.Add(productLine);

                    // ✅ Update Inventory Stock
                 
                }

                // ✅ Update DrugRequest status
                var request = _context.DrugRequests
                    .FirstOrDefault(d => d.DrugRequestId == model.DrugRequestId);

                if (request != null)
                    request.RequestStatus = "Ordered";

                _context.SaveChanges();
                transaction.Commit();

                return RedirectToAction("ViewPurchaseOrders");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        public IActionResult ViewPurchaseOrders(int page = 1)
        {
            var auth = RequireChemist();
            if (auth != null) return auth;

            int pageSize = 5;

            var query = _context.PurchaseOrderHeaders
                .OrderByDescending(p => p.Podate)
                .AsQueryable();

            int totalRecords = query.Count();

            var list = query
     .Skip((page - 1) * pageSize)
     .Take(pageSize)
     .Select(p => new PurchaseOrderListVM
     {
         Poid = p.Poid,
         Pono = p.Pono,
         SupplierName = p.Supplier.SupplierName,
         Podate = p.Podate,
         Status = p.PoStatus ?? "Pending",
         Source = p.DrugRequestId == null ? "Manual" : "Drug Request",
         LineCount = p.PurchaseProductLines.Count()
     }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return View(list);
        }
        public IActionResult ViewPurchaseOrderDetails(int poId)
        {
            var auth = RequireChemist();
            if (auth != null) return auth;

            var po = _context.PurchaseOrderHeaders
                .Where(p => p.Poid == poId)
                .Select(p => new PurchaseOrderDetailsVM
                {
                    Pono = p.Pono,
                    Podate = p.Podate,
                    Poid = p.Poid,
                    SupplierName = p.Supplier.SupplierName,
                    Lines = p.PurchaseProductLines.Select(l => new PurchaseOrderLineVM
                    {
                        DrugName = l.Drug.DrugTitle,
                        Qty = l.Qty,
                        Note = l.Note
                    }).ToList()
                })
                .FirstOrDefault();

            return View(po);
        }
        public IActionResult ViewInventory(string search, int page = 1)
        {
            var auth = RequireChemist();
            if (auth != null) return auth;

            int pageSize = 8;

            var query = _context.Drugs.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d =>
                    d.DrugTitle.Contains(search) ||
                    d.Dosage.Contains(search));
            }

            int totalRecords = query.Count();

            var drugs = query
                .OrderBy(d => d.DrugTitle)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new
                {
                    d.DrugTitle,
                    d.Dosage,
                    d.StockQuantity,
                    d.Expiry
                })
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            ViewBag.Search = search;

            return View(drugs);
        }
        // ================= MARK ORDER AS RECEIVED =================


        public IActionResult ConfirmDelivery(int id)
        {
            var auth = RequireChemist();
            if (auth != null) return auth;

            var po = _context.PurchaseOrderHeaders
                .FirstOrDefault(p => p.Poid == id);

            if (po != null && po.PoStatus == "Dispatched")
            {
                po.PoStatus = "Delivered";

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

            return RedirectToAction("ViewPurchaseOrders");
        }
      

public IActionResult DownloadInvoice(int poId)
    {
        var auth = RequireChemist();
        if (auth != null) return auth;

        var po = _context.PurchaseOrderHeaders
            .Where(p => p.Poid == poId)
            .Select(p => new
            {
                p.Pono,
                p.Podate,
                Supplier = p.Supplier.SupplierName,
                Lines = p.PurchaseProductLines.Select(l => new
                {
                    Drug = l.Drug.DrugTitle,
                    l.Qty,
                    l.Note
                }).ToList()
            })
            .FirstOrDefault();

        if (po == null)
            return NotFound();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Content().Column(col =>
                {
                    col.Item().Text("Purchase Order Invoice")
                        .FontSize(20).Bold();

                    col.Item().Text($"PO No: {po.Pono}");
                    col.Item().Text($"Date: {po.Podate:dd-MM-yyyy}");
                    col.Item().Text($"Supplier: {po.Supplier}");

                    col.Item().PaddingTop(20);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Drug").Bold();
                            header.Cell().Text("Qty").Bold();
                            header.Cell().Text("Note").Bold();
                        });

                        foreach (var line in po.Lines)
                        {
                            table.Cell().Text(line.Drug);
                            table.Cell().Text(line.Qty.ToString());
                            table.Cell().Text(line.Note ?? "");
                        }
                    });
                });
            });
        });

        var pdfBytes = document.GeneratePdf();

        return File(pdfBytes, "application/pdf", $"{po.Pono}_Invoice.pdf");
    }
}

}
