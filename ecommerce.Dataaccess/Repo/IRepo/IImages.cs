using ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Repo.IRepo
{
    public interface IImages :IRepository<Images>
    {
        void Update(Images image);
    }
}
