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
    public class ProductController : Controller
    {
        private IUnitOfwork _unitOfwork;
        private IWebHostEnvironment _hostEnvironment;
        public ProductController(IUnitOfwork unitOfwork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfwork = unitOfwork;
            _hostEnvironment = hostEnvironment;
        }
        // GET: CategoryController
        public ActionResult Index()
        {
            IEnumerable<Product> list = _unitOfwork.Product.GetAll(includeprops:"Category");
            foreach (var item in list)
            {
                if (item.Qty >0)
                {
                    item.status = "In Stock";
                }
                else
                {
                    item.status = "Out of stock";
                }
            }
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index( IFormCollection collection)
        {
            int id = 0;
            int qty = 0;
            Int32.TryParse(collection["id"],out id );
            Int32.TryParse(collection["qty"], out qty);
           var product = _unitOfwork.Product.Get(x=>x.Id == id );
           product.Qty += qty;
            _unitOfwork.Product.Update(product);
            _unitOfwork.Save();
            TempData["success"] = "Productv Stock Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

            // GET: CategoryController/Details/5
            public ActionResult Details(int id)
        {
            var product = _unitOfwork.Product.Get(x => x.Id == id);
            return View(product);
        }

        // GET: CategoryController/Create
        public ActionResult Upsert(int? id)
        {
            var pvm = new ProductVM();
            var newproduct = new Product();
            pvm.Product = newproduct;
            pvm.Categories=_unitOfwork.Category.GetAll().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Text=x.Name ,Value = (x.Id).ToString()});
            if (id == null || id == 0)
            {
                //create new one 


                return View(pvm);
            }
            else
            {
                //get it for update
                pvm.Product = _unitOfwork.Product.Get(x => x.Id == id , includeprops: "Category");


                return View(pvm);
            }
           
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(ProductVM pvm, IFormFile file)
        {
            string wrootpath = _hostEnvironment.WebRootPath;
            try
            {
                

                if (file != null)
                {
                    //NAMe image with random name 
                    
                    string imgname = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productfilepath = Path.Combine(wrootpath, @"Images\Products");
                    if (!string.IsNullOrEmpty(pvm.Product.ImageUrl))
                    {
                        string oldimagepath = Path.Combine(wrootpath ,pvm.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldimagepath))
                        {
                            System.IO.File.Delete(oldimagepath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(productfilepath, imgname), FileMode.Create))
                    {
                        file.CopyTo(filestream);

                    }
                    pvm.Product.ImageUrl = @"\Images\Products\" + imgname;

                }
                if (pvm.Product.Id == 0)
                {
                    _unitOfwork.Product.Add(pvm.Product);
                    var productcategory = _unitOfwork.Category.Get(x => x.Id == pvm.Product.CategoryId);
                    productcategory.productscount += 1;
                    _unitOfwork.Category.Update(productcategory);
                }
                else
                {
                    _unitOfwork.Product.Update(pvm.Product);

                }
                _unitOfwork.Save();
                TempData["success"] = "Product Updated Successfully";
                return RedirectToAction("Index");

            }
            catch (Exception)
            {

                throw;
            }

        }


        // GET: CategoryController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id != 0 )
            {
                var product = _unitOfwork.Product.Get(x => x.Id == id);
                return View(product);
            }
            return View();
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Product product)
        {
            
                if (product.Id != 0)
                {
                    
                    var productcategory = _unitOfwork.Category.Get(x => x.Id == product.CategoryId);
                    if ( productcategory != null)
                    {
                        productcategory.productscount -= 1;
                        _unitOfwork.Category.Update(productcategory);
                    } 
                   
                    _unitOfwork.Product.Remove(product);
                    _unitOfwork.Save();
                TempData["success"] = "Order Successfully Deleted";
                    return RedirectToAction(nameof(Index));
                }
            TempData["error"] = "Please select Product";
                return View(product);

           
           
        }
    }
}