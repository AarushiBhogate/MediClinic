using Microsoft.AspNetCore.Mvc;

namespace MediClinic.Controllers
{
    public class PatientBaseController : Controller
    {
        protected int? PatientId
        {
            get
            {
                return HttpContext.Session.GetInt32("PatientId");
            }
        }

        protected IActionResult? RequireLogin()
        {
            if (PatientId == null)
            {
                return RedirectToAction("Login", "User");
            }

            return null;
        }
    }
}

