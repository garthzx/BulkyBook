using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;

namespace BulkyBookWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    public class ProductController : Controller {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment) {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        // GET
        // Ex: When user just edited/created a product and then creates/edit another
        // product but goes back to the page using the browser, the notification will
        // still pop up. We want to remove that behavior. We just disable caching for this page.
        [ResponseCache(Duration = 0, NoStore = true)]
        public IActionResult Index() {

            //IEnumerable<CoverType> objectCoverTypeList = _unitOfWork.CoverType.GetAll();
            return View();
        }

        // GET 
        public IActionResult Upsert(int? id) {
            ProductViewModel productVM = new() {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(i => new SelectListItem {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null || id == 0) {
                // create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(productVM);
            }
            else {
                // update product
                productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
                return View(productVM);
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel obj, IFormFile? file) {
            if (ModelState.IsValid) {

                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null) {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    // Existing image in database
                    if (obj.Product.ImageUrl != null) {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(oldImagePath)) {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Copy file that was uploaded inside product folder.
                    using (var fileStreams = new FileStream(
                        Path.Combine(uploads, fileName + extension), FileMode.Create)) {
                        file.CopyTo(fileStreams);
                    }
                    obj.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (obj.Product.Id == 0) {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else {
                    _unitOfWork.Product.Update(obj.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product added successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        // POST
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeletePOST(int? id) {
        //    var obj = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);

        //    if (obj == null) {
        //        return NotFound();
        //    }

        //    _unitOfWork.CoverType.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Cover Type deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() {
            // includeProperties is case sensitive
            var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = productList });
        }

        // POST
        [HttpDelete]
        public IActionResult Delete(int? id) {
            var obj = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);

            if (obj == null) {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.Trim('\\'));
            if (System.IO.File.Exists(oldImagePath)) {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
