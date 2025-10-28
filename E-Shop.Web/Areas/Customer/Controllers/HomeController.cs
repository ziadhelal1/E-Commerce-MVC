using E_Shop.Entities.Models;
using E_Shop.Entities.Repositories;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using X.PagedList;


namespace E_Shop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            //if(@User.FindFirst("name").Value == null)
            //{
            //    return RedirectToPage("/Identity/Account/SignIn");
            //}
            var pageNumber = page ?? 1;
            int pageSize = 8;
            var products = _unitOfWork.Product.GetAll().ToPagedList(pageNumber,pageSize);
            return View(products);
        }
        public IActionResult Details(int ProductId)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                ProductId = ProductId,
                
                Product = _unitOfWork.Product
                .GetFirstOrDefault(x => x.Id == ProductId, IncludeWord: "Category"),
                Count = 1
            };


            return View(shoppingCart);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart CartObj=_unitOfWork.ShoppingCart.GetFirstOrDefault(
                x => x.ApplicationUserId == claim.Value && x.ProductId==shoppingCart.ProductId);
            if (CartObj == null)
            {
                int count=0;
                foreach (var item in _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).ToList())
                {
                    count += item.Count;
                }
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,count)
                  
                  ;
            }
            else 
            {
                
                _unitOfWork.ShoppingCart.IncreasementCount(CartObj, shoppingCart.Count);
                _unitOfWork.Complete();
                int count = 0;
                foreach (var item in _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).ToList())
                {
                    count += item.Count;
                }

                HttpContext.Session.SetInt32(SD.SessionKey,count);
            }
            
            
            return RedirectToAction("Index");   

           
        }
    }
}
