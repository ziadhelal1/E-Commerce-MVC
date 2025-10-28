using E_Shop.Entities.Models;
using E_Shop.Entities.Repositories;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Shop.Web.Areas.Admin.Controllers
{
    [Area ("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {

            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = unitOfWork.Category.GetAll();
            //unitOfWork.Complete();

            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(category);
                unitOfWork.Complete();

                TempData["Create"] = "Data has Seen Created Succesfully";
                return RedirectToAction("Index");
            }
            return View(category);

        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }
            var category = unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            //var category = context.Categories.Find(id);
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult edit(Category category)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(category);
                unitOfWork.Complete();
                //context.Categories.Update(category);
                //context.SaveChanges();
                TempData["Update"] = "Data has Seen Updated Succesfully";
                return RedirectToAction("Index");
            }
            return View(category);

        }
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }
            // var category = context.Categories.Find(id);
            var category = unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            //if (id == null || id == 0) { return NotFound(); }
            //var category = context.Categories.Find(id);
            var category = unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            if (category == null) { return NotFound(); }

            //context.Categories.Remove(category);
            //context.SaveChanges();
            unitOfWork.Category.Remove(category);
            unitOfWork.Complete();
            TempData["Delete"] = "Data has Seen Deleted Succesfully";
            return RedirectToAction("Index");
        }
    }
}
