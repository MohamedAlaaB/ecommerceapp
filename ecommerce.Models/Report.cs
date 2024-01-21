using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class Report
    {
       public List<string> products { get; set; } = new List<string>();
        public List<int> counts { get; set; } = new List<int> {};
       public List<Order> order { get; set; }
        public IEnumerable<SelectListItem> Dates { get; set; }   = new List<SelectListItem>();

        public List<string> employeename { get; set; }

        public string? Date { get; set; }
     

        
    }
}
