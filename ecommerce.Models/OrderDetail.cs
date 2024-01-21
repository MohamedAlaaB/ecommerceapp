using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Ordered_qty { get; set; }
        public int order_total_price {get; set; }
        
        public int Orderid { get; set; }
        [ForeignKey("Orderid")]
        public Order order { get; set; }
    }
}
