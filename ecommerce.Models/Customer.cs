using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class Customer
    {
        public int Id { get; set; }
        [Required]
        public string Customer_Name { get; set; }
        [Required]
        public int Customer_PhoneNumber { get; set; }
        [Required]
        public string Customer_Address { get; set;}

        public ICollection<Order> Orders { get; set; }
        
    }
}
