
namespace MediClinic.Models.ModelViews
{
    public class RegisterVM
    {
        // User Table
        public string UserName { get; set; }
        public string Password { get; set; }

        // Role
        public string Role { get; set; }  // Patient / Physician

        // Common fields
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Patient only
        public DateOnly? DOB { get; set; }

        public string Gender { get; set; }
        public string Address { get; set; }

        // Physician only
        public string Specialization { get; set; }
        public string Summary { get; set; }
    }
}
