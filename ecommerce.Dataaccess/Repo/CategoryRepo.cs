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
    public class CategoryRepo : Repository<Category>, ICategory
    {
        private readonly Ecommdbcontext _context;
        public CategoryRepo(Ecommdbcontext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            _context.categories.Update(category);
        }
    }
}
