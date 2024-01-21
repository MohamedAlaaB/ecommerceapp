using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class Calcvm
    {
        public IEnumerable<SelectListItem> Payment_Method { get; set; }
        public IEnumerable<SelectListItem> Order_status { get; set; }
        public Order Order { get; set; }
    }
}
