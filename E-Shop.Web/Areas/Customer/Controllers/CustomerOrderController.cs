using E_Shop.DataAccess.Implementation;
using E_Shop.Entities.Repositories;
using E_Shop.Entities.ViewModels;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace E_Shop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerOrderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;


       



        public CustomerOrderController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;

        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var ordersForCustomer = _unitofwork.OrderHeader
                .GetAll(o => o.ApplictionUserId == claim.Value, IncludeWord: "ApplicationUser");
            return View(ordersForCustomer);
        }
        public IActionResult Details(int id) 
        {
            OrderVM OrderVM = new OrderVM()
            {
                OrderHeader = _unitofwork.OrderHeader
               .GetFirstOrDefault(o => o.Id == id, IncludeWord: "ApplicationUser"),
                OrderDetails = _unitofwork.OrderDetail
               .GetAll(o => o.OrderId == id, IncludeWord: "Product")

            };
            return View(OrderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(OrderVM OrderVM)
        {
            var orderFromDb = _unitofwork.OrderHeader
                .GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);

            orderFromDb.Name = OrderVM.OrderHeader.Name;
            orderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderFromDb.Address = OrderVM.OrderHeader.Address;
            orderFromDb.City = OrderVM.OrderHeader.City;
           
            _unitofwork.OrderHeader.Update(orderFromDb);
            _unitofwork.Complete();
            TempData["Update"] = "Data has Been Updated Succesfully";
            return RedirectToAction("Details", "CustomerOrder", new { id = orderFromDb.Id });

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var orderFromDb = _unitofwork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            if (orderFromDb.PaymentStatus != null)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntentId
                };
                var service = new RefundService();
                Refund refund = service.Create(option);
                _unitofwork.OrderHeader.UpdateOrderStatus(id, SD.Canceled, SD.Refund);

            }
            else
            {
                _unitofwork.OrderHeader.UpdateOrderStatus(id, SD.Canceled, SD.Canceled);

            }
            _unitofwork.Complete();




            TempData["Delete"] = "Order has been Cancelled Successfully ";
            return RedirectToAction("Index", new { id = id });

        }
    }
    
}
