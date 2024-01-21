using ecommerce.Dataaccess.Data;
using ecommerce.Dataaccess.Repo.IRepo;
using ecommerce.Models;
using ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;


namespace ecommerceapp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Supervisor_Role + "," + SD.Admin_Role)]

    public class ReportsController : Controller
    {
        private readonly IUnitOfwork _unitOfwork;
        private readonly Ecommdbcontext _context;
        public ReportsController( IUnitOfwork unitOfwork, Ecommdbcontext context)
        {
                _unitOfwork = unitOfwork;
            _context = context;

        }
        // GET: ReportsController
        public ActionResult Index(string? date , DateTime? start, DateTime? end)
        {
            var AllOrders = new List<Order>();
            var today = DateTime.Now;
            var report = new Report
            {
                order = AllOrders,
                Dates = new List<SelectListItem>
                {
                    new SelectListItem{Text="Today",Value=SD.Daily_Report},
                    new SelectListItem{Text="This Week",Value=SD.Weekly_Report},
                    new SelectListItem{Text="This Month",Value=SD.Monthly_Report},
                    new SelectListItem{Text="Last 3 Month",Value=SD.Quatro_Report},
                    new SelectListItem{Text="Last 6 year",Value=SD.Half_Report}
                },
                employeename = new List<string>()
            };
            if (date != null)
            {
                if (date == SD.Daily_Report)
                {
                    report.order = _context.orders.Where(x => x.creation_date == today).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();

                }
                 
                else if (date == SD.Weekly_Report)
                {
                    report.order = _context.orders.Where(x => x.creation_date <= today && x.creation_date >= today.AddDays(-7)).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                    
                }
                else if (date == SD.Monthly_Report)
                {

                    report.order = _context.orders.Where(x => x.creation_date.Month == today.Month && x.creation_date.Year == today.Year).Include(c => c.Customer).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                }
                else if (date == SD.Quatro_Report)
                {

                    report.order = _context.orders.Where(x => x.creation_date.Month <= today.Month && x.creation_date >= today.AddMonths(-3) && x.creation_date.Year == today.Year).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                }
                else if (date == SD.Half_Report)
                {
                    report.order = _context.orders.Where(x => x.creation_date.Month <= today.Month && x.creation_date >= today.AddMonths(-6) && x.creation_date.Year == today.Year).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                }
                else if (date == null || string.IsNullOrEmpty(date) || date == "select time range" && start ==null)

                {
                    report.order = _context.orders.Where(x => x.Order_Status == SD.Order_Confirmed).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                }
                else if (start != null && end != null &&  date == "select time range")
                {
                    report.order = _context.orders.Where(x => x.creation_date <= end && x.creation_date >= start ).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
                }

            }
            
            else 
           
            {
                report.order = AllOrders = _context.orders.Where(x => x.Order_Status == SD.Order_Confirmed).Include(x => x.OrderDetails).ThenInclude(g => g.Product).ToList();
            }


           
            foreach (var item in report.order)
            {
                report.employeename.Add(item.employeename);
                foreach (var detail in item.OrderDetails)
                {
                    report.products.Add(detail.Product.Name);

                    report.counts.Add(detail.Ordered_qty);
                }
            }

            return View(report);
        }

        // GET: ReportsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ReportsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ReportsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(IFormCollection collection)
        {
            var startst = collection["start"][0];
            var endst = collection["end"][0];
            var Date = collection["Date"][0];
            var Start = new DateTime();
            var End = new DateTime();
            if (!string.IsNullOrEmpty(startst)  && !string.IsNullOrEmpty(endst))
            {
                Start = Convert.ToDateTime(startst);
                End = Convert.ToDateTime(endst);
                return RedirectToAction(nameof(Index), new { date = Date, start = Start, end = End });
            }
           
           
            if (!string.IsNullOrEmpty(Date))

            {
                
                return RedirectToAction(nameof(Index),new { date = Date });
            }
            else
            {
                return RedirectToAction(nameof(Index) );
            }
        }

        // GET: ReportsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReportsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReportsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ReportsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
