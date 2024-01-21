using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Repo.IRepo
{
   public interface IUnitOfwork
    {
        IProduct Product { get; }
        ICategory Category { get; }
        IImages Images { get; }
        IOrder Order { get; }
        ICustomer Customer { get; }
        IOrderDetail OrderDetail { get; }
        void Save();
    }
}
