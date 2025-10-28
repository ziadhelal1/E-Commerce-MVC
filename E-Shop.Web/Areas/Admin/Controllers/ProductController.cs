using E_Shop.Entities.Models;
using E_Shop.Entities.Repositories;
using E_Shop.Entities.ViewModels;
using E_Shop.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Shop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {

            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var products = unitOfWork.Product.GetAll(IncludeWord: "Category");
            //unitOfWork.Complete();

            return View(products);
        }
        public IActionResult GetData()
        {
            var products = unitOfWork.Product.GetAll();
            return Json(new { data = products });
        }
        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                product = new Product(),
                CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                })
            };

            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile file)
        {
            productVM.CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            });
            ModelState.Remove("file");
            if (ModelState.IsValid)
            {
                string RootPath = webHostEnvironment.WebRootPath;
                
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);
                    if (ext==".jpg"||ext==".png")
                    {
                        using (var filestream = new FileStream(Path.Combine(upload, filename + ext), FileMode.Create))
                        {
                            file.CopyTo(filestream);
                        }
                        productVM.product.Img = @"Images\Products\" + filename + ext;
                    }
                    else
                    {
                        ModelState.AddModelError("", "The photo must be .png or .jpg ");

                        return View(productVM);
                    }



                }
                unitOfWork.Product.Add(productVM.product);
                unitOfWork.Complete();

                TempData["Create"] = "Data has Seen Created Succesfully";
                return RedirectToAction("Index");
            }
            return View(productVM);

        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) { return NotFound(); }
            //  var product = unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            //var product = context.Categories.Find(id);
            ProductVM productVM = new ProductVM()
            {
                product = unitOfWork.Product.GetFirstOrDefault(x => x.Id == id),
                CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                })
            };

            return View(productVM);


            //  return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {

                productVM.CategoryList = unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                });
              
                if (file != null)
                {
                    string RootPath = webHostEnvironment.WebRootPath;
                    string filename = Guid.NewGuid().ToString();
                    var upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);
                    if (ext == ".jpg" || ext == ".png")
                    {
                        if (productVM.product.Img != null)
                        {
                            var oldImg = Path.Combine(RootPath, productVM.product.Img.TrimStart('\\'));
                            if (System.IO.File.Exists(oldImg))
                            {
                                System.IO.File.Delete(oldImg);
                            }
                            using (var filestream = new FileStream(Path.Combine(upload, filename + ext), FileMode.Create))
                            {
                                file.CopyTo(filestream);

                            }
                            productVM.product.Img = @"Images\Products\" + filename + ext;
                        }
                    }
                    else 
                    {
                        ModelState.AddModelError("", "The photo must be .png or .jpg ");

                        return View(productVM);
                    }
                       
                   


                }

                unitOfWork.Product.Update(productVM.product);
                unitOfWork.Complete();
                //context.Categories.Update(product);
                //context.SaveChanges();
                TempData["Update"] = "Data has Seen Updated Succesfully";
                return RedirectToAction("Index");
            }
            return View(productVM);

        }

        [HttpDelete]
        
        public IActionResult DeleteProduct(int id)
        {
            //if (id == null || id == 0) { return NotFound(); }
            //var product = context.Categories.Find(id);
            var productFromDb= unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (productFromDb == null) 
            { 
               return Json(new {success=false,message="Error While Deleting "});
            }
            if (productFromDb.Img!=null)
            {
                var oldImg = Path.Combine(webHostEnvironment.WebRootPath, productFromDb.Img.TrimStart('\\'));
                if (System.IO.File.Exists(oldImg))
                {
                    System.IO.File.Delete(oldImg);
                }
            }
           
            
            unitOfWork.Product.Remove(productFromDb);
            unitOfWork.Complete();
            TempData["Delete"] = "Data has Seen Deleted Succesfully";
            return Json(new { success = true, message = "The Item Has Been Deleted Successffully" });
            
        }
    }
}
