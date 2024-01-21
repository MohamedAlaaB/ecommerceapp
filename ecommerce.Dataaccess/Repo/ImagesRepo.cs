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
    public class ImageRepo : Repository<Images>, IImages
    {
        private readonly Ecommdbcontext _context;
        public ImageRepo(Ecommdbcontext context) : base(context)
        {
            _context = context;
        }

        public void Update(Images image)
        {
            _context.images.Update(image);
        }
    }
}
