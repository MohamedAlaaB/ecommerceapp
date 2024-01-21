using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string Order_Status { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
            
        [Required]
        public List<OrderDetail> OrderDetails { get; set; }

        public int CustomerId { get; set; }
        [ForeignKey ("CustomerId")]
        public Customer Customer { get; set; }
        public DateTime creation_date { get; set; } = DateTime.Now;

        public string? employeename { get; set; }
        public int Total { get; set; }
    }
}
