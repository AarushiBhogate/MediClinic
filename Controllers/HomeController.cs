using System.Diagnostics;
using MediClinic.Models;
using Microsoft.AspNetCore.Mvc;
using MediClinic.Models.ModelViews;
namespace MediClinic.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var model = new LandingVM
            {
                Specializations = new List<SpecializationVM>
        {
            new SpecializationVM { Name = "Cardiology", DoctorCount = 4 },
            new SpecializationVM { Name = "Dermatology", DoctorCount = 3 },
            new SpecializationVM { Name = "Pediatrics", DoctorCount = 2 }
        },

                TopDoctors = new List<DoctorCardVM>
        {
            new DoctorCardVM
            {
                Id = 1,
                Name = "Dr. Aisha Mehta",
                Specialization = "Cardiology",
                Experience = 12,
                ImageUrl = "https://randomuser.me/api/portraits/women/44.jpg"
            },
            new DoctorCardVM
            {
                Id = 2,
                Name = "Dr. Rahul Sharma",
                Specialization = "Orthopedics",
                Experience = 10,
                ImageUrl = "https://randomuser.me/api/portraits/men/32.jpg"
            },
            new DoctorCardVM
            {
                Id = 3,
                Name = "Dr. Priya Nair",
                Specialization = "Dermatology",
                Experience = 8,
                ImageUrl = "https://randomuser.me/api/portraits/women/65.jpg"
            }
        }
            };

            return View(model);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
