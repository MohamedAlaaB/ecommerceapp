
using ecommerce.Dataaccess.Data;
using ecommerce.Dataaccess.Migrations;
using ecommerce.Dataaccess.Repo;
using ecommerce.Dataaccess.Repo.IRepo;
using ecommerce.Models;
using ecommerce.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Runtime.Intrinsics.X86;

namespace ecommerceapp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private IUnitOfwork _unitOfwork;
        private Ecommdbcontext _context;

        private int? _id;

        public HomeController(IUnitOfwork unitOfwork, Ecommdbcontext context)
        {
            _unitOfwork = unitOfwork;
            _context = context;
        }

        public IActionResult Index(int? categoryid )
        {
          
            var pos = new Posvm { Categories=_unitOfwork.Category.GetAll(),Products=_unitOfwork.Product.GetAll()};
            int? orderid = HttpContext.Session.GetInt32(SD.Order_id);
            int? filterid = HttpContext.Session.GetInt32(SD.filter_id);
            if (categoryid == -1)
            {
                pos.Products = _unitOfwork.Product.GetAll().Where(x =>  x.Qty == 0);
               
                    
             
            }
            else
            {
                if (categoryid == null && filterid != null && filterid !=0)
                {
                    pos.Products = _unitOfwork.Product.GetAll(x => x.CategoryId == filterid && x.Qty > 0);
                }
                else if (categoryid != null && filterid == null && categoryid != 0)
                {

                    pos.Products = _unitOfwork.Product.GetAll(x => x.CategoryId == categoryid && x.Qty > 0);
                    HttpContext.Session.SetInt32(SD.filter_id, (int)categoryid);

                }
                else if (categoryid != null && filterid != null && categoryid !=0 )
                {
                    pos.Products = _unitOfwork.Product.GetAll(x => x.CategoryId == categoryid && x.Qty > 0);
                    HttpContext.Session.SetInt32(SD.filter_id, (int)categoryid);
                }
                else
                {
                    pos.Products = _unitOfwork.Product.GetAll(x => x.Qty > 0);
                    HttpContext.Session.SetInt32(SD.filter_id, 0);

                }
            }
            List<string> Status = new List<string>() { SD.Order_Confirmed, SD.Order_Returned };
            List<string> Payment = new List<string>() { SD.Payment_Card, SD.Payment_Cash };
            pos.Calcvm = new Calcvm()
            {
                Order =new Order(),
                Order_status = Status.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = x, Text = x }),
                Payment_Method = Payment.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem() { Value = x, Text = x })
            };
            if (orderid > 0)
            {
               
                pos.Calcvm.Order = _context.orders.Where(x => x.Id == orderid).Include(c=>c.Customer).Include(x => x.OrderDetails).ThenInclude(g=>g.Product).FirstOrDefault();

            }
            return View(pos);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public IActionResult Index(Posvm pos)
        {
            
            var orderfromdb = _context.orders.Where(x => x.Id == pos.Calcvm.Order.Id).Include(x => x.Customer).FirstOrDefault();
             if (string.IsNullOrEmpty(pos.Calcvm.Order.PaymentMethod) && string.IsNullOrEmpty(pos.Calcvm.Order.Order_Status))
            {
                orderfromdb.PaymentMethod = SD.Payment_Cash;
                orderfromdb.Order_Status = SD.Order_Confirmed;
            }
            else if (string.IsNullOrEmpty(pos.Calcvm.Order.PaymentMethod) || string.IsNullOrEmpty(pos.Calcvm.Order.Order_Status))
            {
                if (string.IsNullOrEmpty(pos.Calcvm.Order.PaymentMethod))
                {
                    orderfromdb.PaymentMethod = SD.Payment_Cash;
                }
                else
                {
                    orderfromdb.Order_Status = SD.Order_Confirmed;
                }

            }
            else
            {
                orderfromdb.PaymentMethod = pos.Calcvm.Order.PaymentMethod;
                orderfromdb.Order_Status = pos.Calcvm.Order.Order_Status;
            }
           

             _unitOfwork.Order.Update(orderfromdb);
            _unitOfwork.Save();
            HttpContext.Session.Clear();
            TempData["success"] = "Invoice Saved ";
            return RedirectToAction(nameof(Index));
            
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult addtoorder(int ProductId)
        {
           
            
            var productfromdb = _unitOfwork.Product.Get(x => x.Id == ProductId);
            int? orderid = HttpContext.Session.GetInt32(SD.Order_id);
            
           
            if (productfromdb == null || !checkProductStock(productfromdb.Id))
            {
                return RedirectToAction("Index");

            }
            var orderdetail = new OrderDetail()
            {
                ProductId = ProductId,
                Ordered_qty = 1,
                order_total_price = productfromdb.Price

            };
            var customer = new ecommerce.Models.Customer()
            {
                Customer_Address = " "
                ,
                Customer_Name = " ",
                Customer_PhoneNumber = 0000000

            };
           
            if (orderid == null || orderid == 0)
            {
                _unitOfwork.OrderDetail.Add(orderdetail);
                var order = new Order()
                {
                    creation_date = DateTime.Now,
                    Order_Status = SD.Order_UnConfirmed,
                    PaymentMethod = "",
                    OrderDetails = new List<OrderDetail> { orderdetail },
                    Total = orderdetail.order_total_price
                    , Customer = customer
                    , employeename = User.Identity.Name,
                    
                };

                _unitOfwork.Order.Add(order);
               
                _unitOfwork.Save();
                var order2 = _context.orders.Where(x => x.Id == order.Id).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
                order2.OrderDetails[0].Product.Qty--;
                _unitOfwork.Order.Update(order2);
                HttpContext.Session.SetInt32(SD.Order_id, order.Id);
            }
            else if (orderid != 0 && orderid != null)
            {
                var order = _context.orders.Where(x => x.Id == orderid).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
              
                if (order.OrderDetails == null)
                {
                    order.OrderDetails = new List<OrderDetail>() { orderdetail };
                    order.OrderDetails[0].Product.Qty--;
                }
                else
                {
                    if (order.OrderDetails != null)
                    {
                       var detailcontaintheproduct = order.OrderDetails.Where(x=>x.ProductId == productfromdb.Id).FirstOrDefault();
                        if (detailcontaintheproduct != null)
                        {
                            
                            order.OrderDetails.Find(x=>x.Id== detailcontaintheproduct.Id).Ordered_qty += 1;
                            order.OrderDetails.Find(x => x.Id == detailcontaintheproduct.Id).order_total_price +=productfromdb.Price;
                            order.OrderDetails.Find(x => x.Id == detailcontaintheproduct.Id).Product.Qty -= 1;
                            order.Total += detailcontaintheproduct.Product.Price;
                      
                        }else if (detailcontaintheproduct == null)
                        {
                            
                            order.OrderDetails.Add(orderdetail);
                            order.OrderDetails[0].Product.Qty--;
                            order.Total += orderdetail.order_total_price;
                        }
                                              
                    }
                     
                    

                }

                _unitOfwork.Order.Update(order);
               
            }

           
            _unitOfwork.Save();
           
            return RedirectToAction(nameof(Index));
        }
        

            public IActionResult Plus(int orderdetailid)
        {
            var orderDetailFromDb = _unitOfwork.OrderDetail.Get(u => u.Id == orderdetailid,includeprops:"Product");
            var order = _context.orders.Where(x => x.Id == orderDetailFromDb.Orderid).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            if (!checkProductStock(orderDetailFromDb.ProductId))
            {
                return RedirectToAction(nameof(Index));
            }

            order.OrderDetails.Find(x => x.Id == orderdetailid).Ordered_qty += 1;
            order.OrderDetails.Find(x => x.Id == orderdetailid).order_total_price += orderDetailFromDb.Product.Price;
            order.OrderDetails.Find(x => x.Id == orderdetailid).Product.Qty--;
            order.Total += orderDetailFromDb.Product.Price;
                _unitOfwork.Order.Update(order);
            

            _unitOfwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int orderdetailid)
        {
            var orderDetailFromDb = _unitOfwork.OrderDetail.Get(u => u.Id == orderdetailid, includeprops: "Product");
            var order = _context.orders.Where(x => x.Id == orderDetailFromDb.Orderid).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            order.OrderDetails.Find(x => x.Id == orderdetailid).Product.Qty++;
            if (orderDetailFromDb.Ordered_qty <= 1)
            {
                //remove that from cart
                order.OrderDetails.Remove(orderDetailFromDb);
                _unitOfwork.OrderDetail.Remove(orderDetailFromDb);
              
            }
            else
            {
                order.OrderDetails.Find(x=>x.Id == orderdetailid).Ordered_qty -= 1;
                order.OrderDetails.Find(x => x.Id == orderdetailid).order_total_price -= orderDetailFromDb.Product.Price;
                order.Total -= orderDetailFromDb.Product.Price;
                _unitOfwork.Order.Update(order);
            }

            _unitOfwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int orderdetailid)
        {
            var orderDetailFromDb = _unitOfwork.OrderDetail.Get(u => u.Id == orderdetailid);
            var order =_context.orders.Where(x => x.Id == orderDetailFromDb.Orderid).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            order.OrderDetails.Find(x => x.Id == orderdetailid).Product.Qty++;
            //remove that from cart
            int i=order.OrderDetails.FindIndex(x=>x.Id == orderdetailid);
            order.Total -= orderDetailFromDb.order_total_price;
            order.OrderDetails.RemoveAt(i);
            //_unitOfwork.OrderDetail.Remove(orderDetailFromDb);
           
            _unitOfwork.Order.Update(order);
            _unitOfwork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult UpsertCustomer(int? id )
        {
            if (id != null)
            {
                var customer =_unitOfwork.Customer.Get(u => u.Id == id);
              
                return View(customer);
            }
           
            int? orderid = HttpContext.Session.GetInt32(SD.Order_id);
            var orderfromdb = _context.orders.Where(x => x.Id == orderid).Include(x => x.Customer).FirstOrDefault();

           


            return View(orderfromdb.Customer);
          
            
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpsertCustomer(ecommerce.Models.Customer customer)
        {
            int? orderid = HttpContext.Session.GetInt32(SD.Order_id);
            var orderfromdb = _context.orders.Where(x => x.Id == orderid).Include(x => x.Customer).FirstOrDefault();

            try
            {
                if (customer.Customer_PhoneNumber != null)
                {
                    if (checkphonenumbexist(customer.Customer_PhoneNumber))
                    {
                        var thecustomer = _unitOfwork.Customer.Get(x => x.Customer_PhoneNumber == customer.Customer_PhoneNumber);
                        if (thecustomer != null)
                        {
                            if (!string.IsNullOrWhiteSpace(customer.Customer_Address)) 
                            { 
                                orderfromdb.Customer = thecustomer;
                                orderfromdb.Customer.Customer_Address = customer.Customer_Address;
                                _unitOfwork.Order.Update(orderfromdb);
                                _unitOfwork.Save();
                                
                            }
                            return RedirectToAction(nameof(UpsertCustomer), new { id = thecustomer.Id });
                        }
                        
                        
                    }
                }
                
                if (customer.Id == 0)
                {
                    if (!string.IsNullOrWhiteSpace(customer.Customer_Name) && !string.IsNullOrWhiteSpace(customer.Customer_Address) && customer.Customer_PhoneNumber != 0 && orderfromdb != null)
                    {
                        
                        orderfromdb.Customer.Customer_Name = customer.Customer_Name;
                        orderfromdb.Customer.Customer_Address = customer.Customer_Address;
                        ///check if number doesn't exist already
                        orderfromdb.Customer.Customer_PhoneNumber = customer.Customer_PhoneNumber;

                        _unitOfwork.Order.Update(orderfromdb);
                       
                    }
                    else
                    {
                        TempData["error"] = "Data not valid";
                        return View(customer);
                    }
                }
                if (customer.Id != 0)
                {
                    if (orderfromdb.CustomerId == customer.Id)
                    {
                        orderfromdb.Customer.Customer_Name = customer.Customer_Name;
                        orderfromdb.Customer.Customer_Address = customer.Customer_Address;
                        orderfromdb.Customer.Customer_PhoneNumber = customer.Customer_PhoneNumber;
                        _unitOfwork.Order.Update(orderfromdb);

                    }
                    else
                    {
                        orderfromdb.CustomerId = customer.Id;
                        _unitOfwork.Order.Update(orderfromdb);
                    }

                }

                TempData["success"] = "Customer Updated successfully";
                _unitOfwork.Save();
               
                    return RedirectToAction("Index");
              
            }
            catch (Exception)
            {

                throw;
            }
           
        }
        public IActionResult searchByNum(string num)
        {
            var customer =_unitOfwork.Customer.Get(x=>x.Customer_PhoneNumber == Int32.Parse(num));
            if (customer != null)
            {
              
                return RedirectToAction(nameof(UpsertCustomer), new { id = customer.Id });
            }
            else
            {
                TempData["error"] = "No Customer Found";
                return RedirectToAction(nameof(UpsertCustomer), new { id = 0 });
            }
                

        }

        public IActionResult RefundIndex(int? orderid , int? customerId)
        {
            List<Order> orders = new List<Order>();
            var orderfromdb = _context.orders.Where(x => x.Id == orderid).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            if (orderfromdb != null )
            {
                orders.Add(orderfromdb);
                return View(orders);
            }else
            {
               orders = _context.orders.Where(x => x.CustomerId == customerId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                return View(orders);
            }
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult searchByOrderNum(IFormCollection search)
        {
          
            if (search["searchmethod"] == "order")
            {
                var order = Int32.Parse(search["num"]);
                var orderId = _unitOfwork.Order.Get(x => x.Id == order) == null ? 0 : order;
                if (orderId != 0)
                {
                    return RedirectToAction(nameof(RefundIndex), new { orderid = orderId });
                }
                else
                {
                    TempData["error"] = "No Order Found";
                    return RedirectToAction(nameof(RefundIndex));
                }
            }
            else if(search["searchmethod"] =="phone")
            {
                var phonenum = Int64.Parse(search["num"]);
                var customerid = _unitOfwork.Customer.Get(x => x.Customer_PhoneNumber == phonenum) == null ? 0 : _unitOfwork.Customer.Get(x => x.Customer_PhoneNumber == phonenum).Id;


                if (customerid != 0)
                {
                    return RedirectToAction(nameof(RefundIndex), new { customerId = customerid });
                }
                else {
                    TempData["error"] = "No Order Found";
                    return RedirectToAction(nameof(RefundIndex));

                }
            }
            else
            {
                TempData["error"] = "Please Select Phone Number or Invoice number";
                return RedirectToAction(nameof(RefundIndex));
            }
           
        }
        public IActionResult RefundEntireOrder(int orderId)
        {
            var orderfromdb = _context.orders.Where(x => x.Id == orderId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            if (orderfromdb.Order_Status == SD.Order_Confirmed)
            {
                foreach (var item in orderfromdb.OrderDetails)
                {
                    item.Product.Qty += item.Ordered_qty;
                }
                orderfromdb.Order_Status = SD.Order_Returned;
            }else if (orderfromdb.Order_Status == SD.Order_Returned)
            {
                foreach (var item in orderfromdb.OrderDetails)
                {
                    item.Product.Qty -= item.Ordered_qty;
                }
                orderfromdb.Order_Status = SD.Order_Confirmed;
            }
            _unitOfwork.Order.Update(orderfromdb);
            _unitOfwork.Save();
            TempData["success"] = "Order Successfully Update";
            //_unitOfwork.Order.Remove(orderfromdb);
            //_unitOfwork.Save();
            return RedirectToAction(nameof(RefundIndex), new { orderId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RefundSomeOrderProducts(int orderId,int orderdetailid ,int qty )
        {
            ///if order detail count =1 and hit delete triger delete entire oder detail 
            ///if order has only one order detail and it's count =1 hide delte orderdetail button 
            var orderfromdb = _context.orders.Where(x => x.Id == orderId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            var orderdetail = orderfromdb.OrderDetails.Find(x => x.Id == orderdetailid);
            orderdetail.Ordered_qty -= qty;
            orderdetail.order_total_price -= qty * orderdetail.Product.Price;
            orderfromdb.Total -= qty * orderdetail.Product.Price;
            orderdetail.Product.Qty += qty;
            _unitOfwork.Order.Update(orderfromdb);
            _unitOfwork.Save();
            TempData["success"] = "Order Successfully Updated";
            return RedirectToAction(nameof(RefundIndex),new { orderid= orderId });
        }

        public IActionResult RefundOrderDetail(int orderId, int orderdetailid)
        {
            var orderfromdb = _context.orders.Where(x => x.Id == orderId).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).FirstOrDefault();
            var orderdetail = orderfromdb.OrderDetails.Find(x => x.Id == orderdetailid);
            orderfromdb.Total -= orderdetail.Ordered_qty * orderdetail.Product.Price;
            orderdetail.Product.Qty += orderdetail.Ordered_qty;
            
            orderfromdb.OrderDetails.Remove(orderdetail);
            _unitOfwork.Order.Update(orderfromdb);
          
            _unitOfwork.Save();
            TempData["success"] = "Product Successfully Deleted";
            return RedirectToAction(nameof(RefundIndex), new { orderid = orderId });
        }
        public bool checkProductStock(int productid)
        {
            Product x =_unitOfwork.Product.Get(x=>x.Id == productid);
            if (x.Qty >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkphonenumbexist(int phonenumb)
        {
            var customer = _unitOfwork.Customer.Get(x=>x.Customer_PhoneNumber == phonenumb);
            if (customer != null)
            {
                return true;
            }else { return false; }
        }





    }
}
