using ecommerce.Dataaccess.Data;
using ecommerce.Models;
using ecommerce.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;

namespace ecommerceapp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin_Role)]
    public class UsersController : Controller
    {
        private readonly Ecommdbcontext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(Ecommdbcontext context , UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager) {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: UsersController
        public ActionResult Index(string? userph)
        {
            List<AppUser> users = new List<AppUser>();
            if (userph == null)
            {
                users = _context.appUsers.ToList();
                foreach (var item in users)
                {
                    if (string.IsNullOrEmpty(item.Role))
                    {
                        var role = _userManager.GetRolesAsync(item).GetAwaiter().GetResult().FirstOrDefault();
                        item.Role = role;
                    }
                }

            }
            else
            {
                var requireduser = _context.appUsers.Where(x => x.PhoneNumber == userph).FirstOrDefault();
                if (requireduser != null) { users.Add(requireduser); foreach (var item in users)
                    {
                        if (string.IsNullOrEmpty(item.Role))
                        {
                            var role = _userManager.GetRolesAsync(item).GetAwaiter().GetResult().FirstOrDefault();
                            item.Role = role;
                        }
                    }
                }

            }
            return View(users);
        }

        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {

            return View();
        }

        // GET: UsersController/Create
        public ActionResult Upsert(string? id)
        {
            var uservm = new UserVm();
            uservm.roles = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> (){
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Admin" , Value=SD.Admin_Role},
            new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "Supervisor" , Value = SD.Supervisor_Role },
             new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Text = "employee" , Value=SD.Employee_Role}
            };
            var userfromdb = _context.appUsers.Where(x => x.Id == id).FirstOrDefault();
            if (userfromdb == null)
            {
                uservm.User = new AppUser();
               
            }
            uservm.User = userfromdb;
            return View(uservm);
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upsert(IFormCollection collection)
        {
            try
            {
                var userid = collection["id"];
                var role = collection["role"];
                var phonenum = collection["phone"];
                var branch = collection["branch"];
                var email = collection["email"];
                var name = collection["name"];
                var pass = collection["pass"];
                var userfromdb = new AppUser();
                bool isnew = true;
                if (!string.IsNullOrEmpty(userid))
                {
                    isnew = false;
                    userfromdb = _context.appUsers.First(x => x.Id == userid[0]);
                }
               
                
                
               
                var userrole =_userManager.GetRolesAsync(userfromdb).GetAwaiter().GetResult().First();
                List<string> roles = new List<string>() { SD.Employee_Role,SD.Admin_Role,SD.Supervisor_Role };
                if (role != userrole)
                {
                    if(userrole != null)
                    {
                        _userManager.RemoveFromRoleAsync(userfromdb,userrole).GetAwaiter().GetResult();
                    }
                    _userManager.AddToRoleAsync(userfromdb,role).GetAwaiter().GetResult();
                }
                userfromdb.PhoneNumber = phonenum;
                userfromdb.Email = email;
                userfromdb.UserName = name;
                userfromdb.Branch = branch;

                if (isnew == true)
                {
                   _userManager.AddPasswordAsync(userfromdb,pass).GetAwaiter().GetResult();
                    _context.appUsers.Add(userfromdb);
                }
                else
                {
                    _context.appUsers.Update(userfromdb);

                }

                _context.SaveChanges();
                TempData["success"] = "User Successfully Updated";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["error"] = "Please Select User";
                return View();
            }
        }

       
        // GET: UsersController/Delete/5
        public ActionResult Delete(string id)
        {
           var user = _context.appUsers.First(x => x.Id == id);
            return View(user);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(IFormCollection collection)
        {
            try
            {
                var userid = collection["id"][0];
                if (userid != null)
                {
                    var user =_context.appUsers.First(x=>x.Id == userid);
                    _context.appUsers.Remove(user);
                    _context.SaveChanges();
                    TempData["success"] = "User Deleted Successfully";
                    return RedirectToAction(nameof(Index));

                }else {
                    TempData["error"] = "Please Select User";
                    return View(); }
                
            }
            catch
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult searchByOrderNum(IFormCollection searchVM)
        {

            if (!string.IsNullOrEmpty(searchVM["num"][0] ))
            {
                var user = searchVM["num"].ToString();
                var userPh = (_context.Users.Where(x => x.Id == user) == null ? "0" : user);
                if (userPh != "0")
                {


                    return RedirectToAction(nameof(Index), new { userph = userPh });
                }
                else
                {
                    TempData["error"] = "No User Found";
                    return RedirectToAction(nameof(Index));

                }
            
          
            }
            else
            {
                TempData["error"] = "Please select phonenumber ";
                return RedirectToAction(nameof(Index));
            }


        }

    }
   
}
