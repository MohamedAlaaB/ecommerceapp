using ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Repo.IRepo
{
    public interface IOrderDetail :IRepository<OrderDetail>
    {
        void Update( OrderDetail order);
    }
}
