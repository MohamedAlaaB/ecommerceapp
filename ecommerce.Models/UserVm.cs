using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class UserVm
    {
        public AppUser User {  get; set; }
        public List<SelectListItem> roles { get; set; }
    }
}
