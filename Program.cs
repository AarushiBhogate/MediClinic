using MediClinic.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MediClinic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add MVC
            builder.Services.AddControllersWithViews();

            // Add Session
            builder.Services.AddSession();

            // Add Database
            builder.Services.AddDbContext<MediClinicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add Authentication (Cookie)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                    options.AccessDeniedPath = "/User/AccessDenied";
                });

            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied";
    });

            builder.Services.AddAuthorization();


            var app = builder.Build();
       

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();      // ✅ Important
            app.UseRouting();

            app.UseSession();          // ✅ Keep this before authentication if using session

            app.UseAuthentication();   // ✅ Must come before Authorization
            app.UseAuthorization();
            app.UseAuthentication();
      
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}