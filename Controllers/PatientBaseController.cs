using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace MediClinic.Controllers
{
    public class PatientBaseController : Controller
    {
        protected int? PatientId
        {
            get
            {
                return HttpContext.Session.GetInt32("RoleReferenceID");
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

