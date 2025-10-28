using E_Shop.DataAccess.Data;
using E_Shop.Entities.Models;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace E_Shop.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        
        private readonly UserManager<IdentityUser> _userManager;
       
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
           
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
           
            _roleManager = roleManager;
            _context = context;
        }
        public void Initialize()
        {

            //migrations
            //

            try
            {
                if (_context.Database.GetPendingMigrations().Count()>0)
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }

            //roles

            if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();

                //users


                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName= "admin@admin.com",
                    Email="admin@admin.com",
                    Name="Ziad Helal",
                    PhoneNumber="01091545953",
                    Address="mitassas",
                    City="samnoud"
                    
                },"123456zZ@").GetAwaiter().GetResult();
                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(a => a.Email == "admin@admin.com");
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim("name", user.Name));
                _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();
                _userManager.AddClaimsAsync(user, claims).GetAwaiter().GetResult();
             //   _userManager.AddPasswordAsync(user,"123456").GetAwaiter().GetResult();
            }




        }
    }
}
