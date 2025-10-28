﻿using E_Shop.DataAccess.Data;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles = SD.AdminRole)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
           _context = context;
        }
        public IActionResult Index()
        {
            var claimsIdentity =(ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;
            var customers = _context.ApplicationUsers.Where(x=>x.Id!=userId).ToList();

            return View(customers);
        }
        public IActionResult LockUnlock(string?id)
        {
            var user=_context.ApplicationUsers.FirstOrDefault(x=>x.Id==id);
            if (user==null)
            {
                return NotFound();
            }
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            else 
            {
                user.LockoutEnd= DateTime.Now;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "users", new {area="Admin"});

        }
        public IActionResult Delete(string? id) 
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            _context.ApplicationUsers.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
