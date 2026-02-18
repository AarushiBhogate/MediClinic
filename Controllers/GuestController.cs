using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class GuestController : Controller
{
    private readonly MediClinicDbContext _context;

    public GuestController(MediClinicDbContext context)
    {
        _context = context;
    }

    // List all active drugs
    public IActionResult Index()
    {
        var drugs = _context.Drugs
                            .Where(d => d.DrugStatus == "Active")
                            .ToList();

        return View(drugs);
    }

    // View single drug details
    public IActionResult Details(int id)
    {
        var drug = _context.Drugs.FirstOrDefault(d => d.DrugId == id);

        if (drug == null)
            return NotFound();

        return View(drug);
    }
}
