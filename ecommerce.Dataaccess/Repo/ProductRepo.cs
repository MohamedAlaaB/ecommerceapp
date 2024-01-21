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
    public class ProductRepo : Repository<Product>, IProduct
    {
        private readonly Ecommdbcontext _context;
        public ProductRepo(Ecommdbcontext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            _context.products.Update(product);
        }
    }
}
