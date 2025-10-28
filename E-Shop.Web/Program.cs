using E_Shop.DataAccess.Data;
using E_Shop.DataAccess.DbInitializer;
using E_Shop.DataAccess.Implementation;
using E_Shop.Entities.Repositories;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Shop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();//.AddRazorRuntimeCompilation();
            //builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(
            //    ))
            builder.Services.AddDbContext<ApplicationDbContext>(options =>  options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("Stripe"));
            builder.Services.AddIdentity<IdentityUser, IdentityRole>
            (

                options => { options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(2);
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;

                }



            )
                .AddDefaultTokenProviders().AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            // builder.Services.AddDefaultIdentity<IdentityUser>(options=>options.SignIn.RequireConfirmedAccount=false)
            //.AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.AddDistributedMemoryCache();  
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
            SeedDb();
            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages();

            app.MapControllerRoute(
               name: "default",
               pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

           
            app.Run();
            void SeedDb()
            {
                using (var scope=app.Services.CreateScope())
                {
                    var dbIntialzer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                    dbIntialzer.Initialize();
                }
            }
        }
    }
}
