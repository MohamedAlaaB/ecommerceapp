using ecommerce.Dataaccess.Data;
using ecommerce.Dataaccess.Repo.IRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Repo
{
    public class UnitOfwork : IUnitOfwork
    {
        public IProduct Product { get; private set; }

        public ICategory Category { get; private set; }
        public IImages Images { get; private set; }
        public IOrder Order { get; private set; }
        public IOrderDetail OrderDetail { get; private set; }

        public ICustomer Customer { get; private set; }
        private Ecommdbcontext _context;
        public UnitOfwork( Ecommdbcontext context ) 
        {
            _context = context;
            Product = new ProductRepo(_context);
            Category = new CategoryRepo(_context);
            Images = new ImageRepo(_context);
            Order = new OrderRepo(_context);
            OrderDetail = new OrderDtailRepo(_context);
            Customer = new CustomerRepo(_context);
        }
        public void Save()
        {
           _context.SaveChanges();
        }
    }
}
