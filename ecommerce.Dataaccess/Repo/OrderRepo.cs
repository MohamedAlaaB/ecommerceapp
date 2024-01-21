using ecommerce.Dataaccess.Data;
using ecommerce.Dataaccess.Repo.IRepo;
using ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Repo
{
    public class OrderRepo : Repository<Order>, IOrder
    {
        private readonly Ecommdbcontext _context;
        public OrderRepo(Ecommdbcontext context) : base(context)
        {
            _context = context;
        }

        public void Update(Order order)
        {
            _context.orders.Update(order);
        }
    }
}
