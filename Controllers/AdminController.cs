using Microsoft.AspNetCore.Mvc;

namespace MediClinic.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
