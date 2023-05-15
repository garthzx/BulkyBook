using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;

namespace BulkyBookWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    public class CompanyController : Controller {

        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
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
            Company company = new();

            if (id == null || id == 0) {
                // create product
                //ViewBag.CategoryList = CategoryList;
                //ViewData["CoverTypeList"] = CoverTypeList;
                return View(company);
            }
            else {
                // update product
                company = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);
                return View(company);
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj, IFormFile? file) {
            if (ModelState.IsValid) {
                if (obj.Id == 0) {
                    _unitOfWork.Company.Add(obj);
                    TempData["success"] = "Company added successfully";
                }
                else {
                    _unitOfWork.Company.Update(obj);
                    TempData["success"] = "Company updated successfully";
                }

                _unitOfWork.Save();
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
            var companyList = _unitOfWork.Company.GetAll();
            return Json(new { data = companyList });
        }

        // POST
        [HttpDelete]
        public IActionResult Delete(int? id) {
            var obj = _unitOfWork.Company.GetFirstOrDefault(u => u.Id == id);

            if (obj == null) {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
