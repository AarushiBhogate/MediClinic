//using MediClinic.Models;
//using Microsoft.EntityFrameworkCore;
//namespace MediClinic
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {

//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.
//            builder.Services.AddControllersWithViews();

//            builder.Services.AddDbContext<MediClinicDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//            }
//            //app.UseRouting();

//            //app.UseAuthorization();

//            //app.MapStaticAssets();
//            //app.MapControllerRoute(
//            //    name: "default",
//            //    pattern: "{controller=Home}/{action=Index}/{id?}")
//            //    .WithStaticAssets();
//            app.UseStaticFiles();

//            app.UseRouting();

//            app.UseSession();

//            app.UseAuthorization();

//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}");


//            app.Run();
//        }
//    }
//}

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

            builder.Services.AddControllersWithViews();

            // DB Connection
            builder.Services.AddDbContext<MediClinicDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            // ✅ SESSION CONFIG (Required)
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession();

            builder.Services.AddHttpContextAccessor();

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

            app.UseStaticFiles();

            app.UseRouting();

            // ✅ Enable Session
            app.UseSession();

            app.UseAuthorization();
            app.UseAuthentication();
      
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

