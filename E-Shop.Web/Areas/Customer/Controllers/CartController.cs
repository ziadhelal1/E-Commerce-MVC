using E_Shop.DataAccess.Data;
using E_Shop.DataAccess.Implementation;
using E_Shop.Entities.Models;
using E_Shop.Entities.Repositories;
using E_Shop.Entities.ViewModels;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace E_Shop.Web.Areas.Customer.Controllers
{
    [Area ("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IConfiguration _configuration;

        public ShoppingCartVM ShoppingCartVM { get; set; }

        

        public CartController(IUnitOfWork unitofwork,IConfiguration configuration)
        {
            _unitofwork = unitofwork;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = _unitofwork.ShoppingCart.GetAll
                (s => s.ApplicationUserId == claim.Value, IncludeWord: "Product")
            };
            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.TotalPrice += item.Count * item.Product.Price;
                ShoppingCartVM.TotalQuantity += item.Count;
            }
            return View(ShoppingCartVM);
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = _unitofwork.ShoppingCart.GetAll
               (s => s.ApplicationUserId == claim.Value, IncludeWord: "Product")
            };
            ShoppingCartVM.OrderHeader = new OrderHeader();
           

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofwork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
            ShoppingCartVM.OrderHeader.Name= ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.Email = ShoppingCartVM.OrderHeader.ApplicationUser.Email;
            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += item.Count * item.Product.Price;

            }



            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PostSummary(ShoppingCartVM ShoppingCartVM) 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.CartList = _unitofwork.ShoppingCart
                .GetAll(s=>s.ApplicationUserId==claim.Value ,IncludeWord:"Product");
            


              ShoppingCartVM.OrderHeader.OrederStatus = SD.Pending;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplictionUserId = claim.Value;

           var user = _unitofwork.ApplicationUser.GetFirstOrDefault(a=>a.Id==claim.Value);
            if (user != null) 
            {
                user.PhoneNumber = ShoppingCartVM.OrderHeader.PhoneNumber;
                user.Address= ShoppingCartVM.OrderHeader.Address;
                user.City= ShoppingCartVM.OrderHeader.City;
            }


              
            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Product.Price * item.Count);
            }
            _unitofwork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitofwork.Complete();
            foreach (var item in ShoppingCartVM.CartList)
            {
                OrderDetail orderDetail = new OrderDetail() 
                {
                    ProductId=item.ProductId,
                    OrderId=ShoppingCartVM.OrderHeader.Id,
                    Price=item.Product.Price,
                    Count=item.Count
                };
                _unitofwork.OrderDetail.Add(orderDetail);
                _unitofwork.Complete();



            }
           // var domain = "https://localhost:7087/";
            var domain = _configuration.GetSection("Addresses:DomainAddress").Value;
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
       
                Mode = "payment",
                SuccessUrl = domain+ $"customer/cart/orderconfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl =domain+ $"customer/cart/index",
            };
            foreach (var item in ShoppingCartVM.CartList)
            {

              var SessionLineItemOptions=  new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price*100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,

                };
                options.LineItems.Add(SessionLineItemOptions);
            }

            var service = new SessionService();
            Session session = service.Create(options);
            ShoppingCartVM.OrderHeader.SessionId = session.Id;
            _unitofwork.Complete();

            Response.Headers.Add("Location", session.Url);
              return new StatusCodeResult(303);
    //            return RedirectToAction("index");
        }
        public IActionResult orderconfirmation(int id) 
        {
            OrderHeader orderHeader = _unitofwork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            orderHeader.PaymentIntentId = session.PaymentIntentId;

            if (session.PaymentStatus.ToLower()=="paid")
            {
                _unitofwork.OrderHeader.UpdateOrderStatus(id, SD.Approve, SD.Approve);
               

                _unitofwork.Complete();
            }
            var deletedCartList = _unitofwork.ShoppingCart.GetAll(s=>s.ApplicationUserId==orderHeader.ApplictionUserId).ToList();
            _unitofwork.ShoppingCart.RemoveRange(deletedCartList);
            _unitofwork.Complete();
            return View(id);
        }
        public IActionResult Plus(int cartId) 
        {
            ShoppingCart shoppingCartInDb= _unitofwork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitofwork.ShoppingCart.IncreasementCount(shoppingCartInDb,1);
            _unitofwork.Complete();
            var cartList = _unitofwork.ShoppingCart.GetAll(s => s.ApplicationUserId == shoppingCartInDb.ApplicationUserId).ToList();
            int count = 0;
            foreach (var item in cartList)
            {
                count += item.Count;
            }
            HttpContext.Session.SetInt32(SD.SessionKey, count);
            return RedirectToAction("Index");   

        }
        public IActionResult Minus(int cartId)
        {
            ShoppingCart shoppingCartInDb = _unitofwork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            _unitofwork.ShoppingCart.DecreasementCount(shoppingCartInDb, 1);
            _unitofwork.Complete();
            var cartList = _unitofwork.ShoppingCart.GetAll(s => s.ApplicationUserId == shoppingCartInDb.ApplicationUserId).ToList();
            int count = 0;
            foreach (var item in cartList)
            {
                count += item.Count;
            }
            HttpContext.Session.SetInt32(SD.SessionKey, count);
            return RedirectToAction("Index");

        }
        public IActionResult Remove(int cartId)
        {

            ShoppingCart shoppingCartInDb = _unitofwork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId,IncludeWord:"ApplicationUser");
            _unitofwork.ShoppingCart.Remove(shoppingCartInDb);
            _unitofwork.Complete();
            int count = 0;
            foreach (var item in _unitofwork.ShoppingCart.GetAll(s => s.ApplicationUserId == shoppingCartInDb.ApplicationUserId).ToList())
            {
                count += item.Count;
            }
            HttpContext.Session.SetInt32(SD.SessionKey,count);
            return RedirectToAction("Index");
        }
        
    }
}
