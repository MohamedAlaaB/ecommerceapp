using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class SearchVM
    {
        public List<Order>orders { get; set; }
        public int? query { get; set; }
        public string? SearchingMethod { get; set; }
    }
}
