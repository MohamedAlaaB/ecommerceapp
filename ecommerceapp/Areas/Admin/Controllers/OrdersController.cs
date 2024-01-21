using ecommerce.Dataaccess.Data;
using ecommerce.Dataaccess.Repo.IRepo;
using ecommerce.Models;
using ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecommerceapp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Supervisor_Role+","+SD.Admin_Role)]
    public class OrdersController : Controller
    {
        private IUnitOfwork _unitOfwork;
        private Ecommdbcontext _context;
        public OrdersController(IUnitOfwork unitOfwork , Ecommdbcontext context)
        {
            _context = context;
            _unitOfwork = unitOfwork;
        }
        // GET: OrdersController
        public IActionResult Index(int? orderid, int? customerId)
        {
            var searchvm = new SearchVM();
            searchvm.orders = new List<Order>();
            var orderfromdb = _context.orders.Where(x => x.Id == orderid).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            if (orderfromdb != null)
            {
                searchvm.orders.Add(orderfromdb);
                return View(searchvm);
            }
            else if(customerId != null)
            {
                searchvm.orders = _context.orders.Where(x => x.CustomerId == customerId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                return View(searchvm);
            }
            else
            {
                searchvm.orders = _context.orders.Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                     return View(searchvm);
            }

        }
        [HttpPost]

        // GET: OrdersController/Details/5
        public IActionResult Details(int id)
        {
            return View();
        }

        // GET: OrdersController/Create
        public IActionResult Upsert( int id)
        {
            if (id == 0 )
            {
                //create
                var order = new Order();
                return View(order);
            }
            else
            {
                //edit
                var order = _unitOfwork.Order.Get(x=>x.Id == id);
                return View(order);
            }
           
        }

        // POST: OrdersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Order order)
        {
            try
            {
                if (order.Id == 0)
                {
                    _unitOfwork.Order.Add(order);

                }
                else
                {
                    _unitOfwork.Order.Update(order);
                }
                _unitOfwork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(order);
            }
        }

       
        

        // GET: OrdersController/Delete/5
       



       

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult searchByOrderNum(IFormCollection searchVM)
        {
           
            if (searchVM["SearchMethod"] == "order")
            {
                var order = Int32.Parse(searchVM["num"].ToString());
                var orderId = (_unitOfwork.Order.Get(x => x.Id == order) == null ? 0 : order);
                if (orderId != 0)
                {


                    return RedirectToAction(nameof(Index), new { orderid = orderId });
                }
                else
                {
                    TempData["error"] = "No Order Found";
                    return RedirectToAction(nameof(Index));

                }
            }
            else if (searchVM["SearchMethod"] == "phone")
            {
                var phonenum = Int64.Parse(searchVM["num"].ToString());
                var customerid = (_unitOfwork.Customer.Get(x => x.Customer_PhoneNumber == phonenum) == null ? 0 : _unitOfwork.Customer.Get(x => x.Customer_PhoneNumber == phonenum).Id);

                if (customerid != 0)
                {
                    return RedirectToAction(nameof(Index), new { customerId = customerid });
                }
                else
                {
                    TempData["error"] = "No Order Found";
                    return RedirectToAction(nameof(Index));

                }
            }
            else
            {
                TempData["error"] = "Please select phonenumber or order ";
                return RedirectToAction(nameof(Index));
            }


        }
        public IActionResult DeleteEntireOrder(int orderId)
        {
            var orderfromdb = _context.orders.Where(x => x.Id == orderId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            foreach (var item in orderfromdb.OrderDetails)
            {
                item.Product.Qty += item.Ordered_qty;
            }
            _unitOfwork.Order.Update(orderfromdb);
            _unitOfwork.Save();

            _unitOfwork.Order.Remove(orderfromdb);
            TempData["success"] = "Order Successfully Deleted";
            _unitOfwork.Save();
            return RedirectToAction(nameof(Index), new { orderId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSomeOrderProducts(int orderId, int orderdetailid, int qty)
        {
           
            var orderfromdb = _context.orders.Where(x => x.Id == orderId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            var orderdetail = orderfromdb.OrderDetails.Find(x => x.Id == orderdetailid);
            if (qty > orderdetail.Ordered_qty)
            {
               
                orderdetail.order_total_price += System.Math.Abs(qty - orderdetail.Ordered_qty) * orderdetail.Product.Price;
                orderfromdb.Total += System.Math.Abs(qty - orderdetail.Ordered_qty) * orderdetail.Product.Price;
                orderdetail.Product.Qty -= System.Math.Abs( qty-orderdetail.Ordered_qty);
                orderdetail.Ordered_qty = qty;
            }
            else if (qty < orderdetail.Ordered_qty)
            {
               
                orderdetail.order_total_price -= System.Math.Abs(qty - orderdetail.Ordered_qty) * orderdetail.Product.Price;
                orderfromdb.Total -= System.Math.Abs(qty - orderdetail.Ordered_qty) * orderdetail.Product.Price;
                orderdetail.Product.Qty += System.Math.Abs(qty - orderdetail.Ordered_qty);
                orderdetail.Ordered_qty = qty;
            }
            
            _unitOfwork.Order.Update(orderfromdb);
            TempData["success"] = "Order Successfully Updated";
            _unitOfwork.Save();
            return RedirectToAction(nameof(Index), new { orderid = orderId });
        }

        public IActionResult DeleteOrderDetail(int orderId, int orderdetailid)
        {
            var orderfromdb = _context.orders.Where(x => x.Id == orderId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            var orderdetail = orderfromdb.OrderDetails.Find(x => x.Id == orderdetailid);
            if (orderfromdb.OrderDetails.Count > 1)
            {
                orderfromdb.Total -= orderdetail.Ordered_qty * orderdetail.Product.Price;
                orderdetail.Product.Qty += orderdetail.Ordered_qty;
                orderfromdb.OrderDetails.Remove(orderdetail);
                _unitOfwork.Order.Update(orderfromdb);
            }
            else if (orderfromdb.OrderDetails.Count == 1)
            {
                _unitOfwork.Order.Remove(orderfromdb);
                TempData["success"] = "Product Successfully Deleted";
            }
          
           
            _unitOfwork.Save();
            return RedirectToAction(nameof(Index), new { orderid = orderId });
        }
        public bool checkProductStock(int productid)
        {
            Product x = _unitOfwork.Product.Get(x => x.Id == productid);
            if (x.Qty >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
