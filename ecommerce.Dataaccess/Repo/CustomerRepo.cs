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
    public class CustomerRepo : Repository<Customer>, ICustomer
    {
        private readonly Ecommdbcontext _context;
        public CustomerRepo(Ecommdbcontext context) : base(context)
        {
            _context = context;
        }

        public void Update(Customer customer)
        {
            _context.customers.Update(customer);
        }
    }
}
