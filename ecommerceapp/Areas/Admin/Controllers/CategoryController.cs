using ecommerce.Dataaccess.Repo;
using ecommerce.Dataaccess.Repo.IRepo;
using ecommerce.Models;
using ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerceapp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin_Role)]
    public class CategoryController : Controller
    {
        private  IUnitOfwork _unitOfwork;
        public CategoryController(IUnitOfwork unitOfwork)
        {
                _unitOfwork = unitOfwork;
        }
        // GET: CategoryController
        public ActionResult Index()
        {
            IEnumerable<Category> list = _unitOfwork.Category.GetAll();
            return View(list);
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            var category = _unitOfwork.Category.Get(x=>x.Id==id);
            return View(category);
        }

        // GET: CategoryController/Create
        public ActionResult Upsert(int? id)
        {
            var newcategory = new Category();
            if (id == null || id == 0)
            {
                //create new one 
               
                
                return View(newcategory);
            }
            else
            {
                //get it for update
                var category = _unitOfwork.Category.Get(x => x.Id == id);


                return View(category);
            }
            return View(newcategory); 
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (category.Id == 0 )
                    {
                        _unitOfwork.Category.Add(category);
                    }
                    else
                    {
                       _unitOfwork.Category.Update(category);
                       
                    }
                    TempData["success"] = "Category Successfully Updated";
                    _unitOfwork.Save();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {

                throw;
            }
            TempData["error"] = "Product Select Category";
            return View(category);
        }

     
     
        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            if(id !=0 || id != null)
            {
                var category = _unitOfwork.Category.Get(x => x.Id == id);
                return View(category);
            }
            return View();
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Category category)
        {
            try
            {
                
                    if(category.Id != 0)
                    {
                        _unitOfwork.Category.Remove(category);
                        _unitOfwork.Save();
                    TempData["success"] = "Product Successfully Deleted";
                        return RedirectToAction(nameof(Index));
                    }
               
                return View(category) ;

            }
            catch
            {
                TempData["error"] = "Product Select Category";
                return View(category);
            }
        }
    }
}
