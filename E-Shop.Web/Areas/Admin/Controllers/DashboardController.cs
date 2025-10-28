using E_Shop.Entities.Repositories;
using E_Shop.Entities.ViewModels;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.Web.Areas.Admin.Controllers
{
    [Area ("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {

          _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            DashBoardVM DashBoardVM = new DashBoardVM()
            {
                ProductsCount=_unitOfWork.Product.GetAll().Count(),
                CategoriesCount= _unitOfWork.Category.GetAll().Count(),
                OrdersCount= _unitOfWork.OrderHeader.GetAll().Count(),
                UsersCount= _unitOfWork.ApplicationUser.GetAll().Count()
            };
            return View(DashBoardVM);
        }
    }
}
