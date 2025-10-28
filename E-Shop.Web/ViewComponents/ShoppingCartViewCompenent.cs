﻿
using E_Shop.Entities.Repositories;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Shop.Web.ViewCompenents
{
    public class ShoppingCartViewComponent: ViewComponent

    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }
        public async Task <IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionKey) != null)
                {
                    int count = 0;
                    foreach (var item in _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).ToList())
                    {
                        count += item.Count;
                    }
                    HttpContext.Session.SetInt32(SD.SessionKey, count);
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));
                }
                else
                {
                    int count = 0;
                    foreach (var item in _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).ToList())
                    {
                        count += item.Count;
                    }
                    HttpContext.Session.SetInt32(SD.SessionKey, count);
                    return View(HttpContext.Session.GetInt32(SD.SessionKey));

                }
            }
            else 
            {
                HttpContext.Session.Clear();
                return View(0);
            }

        }
    }
}
