using E_Shop.Entities.Repositories;
using E_Shop.Entities.ViewModels;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;

namespace E_Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]

    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //[BindProperty]
        //OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
           var Orders= _unitOfWork.OrderHeader.GetAll(IncludeWord: "ApplicationUser");
            return View(Orders);
        }
        public IActionResult Details(int id)
        {
            OrderVM OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == id, IncludeWord: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail
                .GetAll(o => o.OrderId == id, IncludeWord: "Product")

            };
            return View(OrderVM);
        }
        public IActionResult UpdateOrderDetails(int id)
        {

            OrderVM OrderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == id, IncludeWord: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail
                .GetAll(o => o.OrderId == id, IncludeWord: "Product")

            };
            return View(OrderVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateOrderDetails(OrderVM OrderVM)
        {
            var orderFromDb = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            
                orderFromDb.Name=OrderVM.OrderHeader.Name;
                orderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
                orderFromDb.Address = OrderVM.OrderHeader.Address;
                orderFromDb.City = OrderVM.OrderHeader.City;
                if (OrderVM.OrderHeader.Carrier!=null)
                {
                    orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
                }
                if (OrderVM.OrderHeader.TrackingNumber != null)
                {
                    orderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
                }
                _unitOfWork.OrderHeader.Update(orderFromDb);
                _unitOfWork.Complete();
                TempData["Update"] = "Data has Been Updated Succesfully";
              return RedirectToAction("UpdateOrderDetails", "Order", new {id=orderFromDb.Id} );
          
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StrartProcessing(int id )
        {
            
            var orderFromDb = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == id);
            _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Procecssing, null);
            _unitOfWork.Complete(); 

          
            
            TempData["Update"] = "Order Status has been Updated Successfully ";
            return RedirectToAction("Details", "Order", new { id = id });

        }
        public IActionResult StrartShipping(OrderVM OrderVM)
        {

            var orderFromDb = _unitOfWork.OrderHeader
                .GetFirstOrDefault(o => o.Id == OrderVM.OrderHeader.Id);
            orderFromDb.TrackingNumber= OrderVM.OrderHeader.TrackingNumber;
            orderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderFromDb.OrederStatus = SD.Shipped;
            orderFromDb.ShippingDate = DateTime.Now;
            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Complete();
            TempData["Update"] = "Order has been Shipped Successfully ";
            return RedirectToAction("Details", "Order", new { id = OrderVM.OrderHeader.Id });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CanelOrder(int id)
        {
            var orderFromDb=_unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            if (orderFromDb.PaymentStatus != null)
            {
                var option = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderFromDb.PaymentIntentId
                };
                var service = new RefundService();
                 Refund refund = service.Create(option);
                _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Canceled, SD.Refund);

            }
            else 
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.Canceled, SD.Canceled);

            }
            _unitOfWork.Complete ();




            TempData["Delete"] = "Order has been Cancelled Successfully ";
            return RedirectToAction("Details", "Order", new { id = id });

        }
    }
}
