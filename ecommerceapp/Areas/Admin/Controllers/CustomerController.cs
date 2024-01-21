using ecommerce.Dataaccess.Data;
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
    public class CustomerController : Controller
    {
        private IUnitOfwork _unitOfwork;
        private Ecommdbcontext _context;
        public CustomerController(IUnitOfwork unitOfwork, Ecommdbcontext context)
        {
            _context = context;
            _unitOfwork = unitOfwork;
        }
        // GET: CustomerController
        public IActionResult Index()
        {
            List<ecommerce.Models.Customer> customers = _unitOfwork.Customer.GetAll();
            return View(customers);
        }

        // GET: CustomerController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: CustomerController/Create
        public IActionResult Upsert(int id)
        {
            if (id == 0)
            {
                //create
                var customer = new ecommerce.Models.Customer();
                return View(customer);
            }
            else
            {
                //edit
                var customer = _unitOfwork.Customer.Get(x => x.Id == id);
                return View(customer);
            }

        }

        // POST: OrdersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ecommerce.Models.Customer customer)
        {
            try
            {
                if (customer.Id == 0)
                {
                    _unitOfwork.Customer.Add(customer);

                }
                else
                {
                    _unitOfwork.Customer.Update(customer);
                }
              
                _unitOfwork.Save();
                TempData["success"] = "Customer Successfully Updated";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["error"] = "Please Select customer";
                return View();
            }
        }


        // GET: CustomerController/Edit/5
       

        // GET: CustomerController/Delete/5
        public IActionResult Delete(int id)
        {
            var customer = _unitOfwork.Customer.Get(x => x.Id == id);
            return View(customer);
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(ecommerce.Models.Customer customer)
        {
            try
            {
                if (customer != null)
                {
                    _unitOfwork.Customer.Remove(customer);
                    _unitOfwork.Save();
                    TempData["success"] = "Customer Successfully Deleted";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = "Please Select customer";
                    return View(customer);
                }

            }
            catch
            {
                return View();
            }
        }
    }
}
