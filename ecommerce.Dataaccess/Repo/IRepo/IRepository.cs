using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Repo.IRepo
{
    public interface IRepository<T> where T : class
    {
       public void Add(T item);
       public T Get(Expression<Func<T,bool>> filter,string? includeprops = null ,bool tracking = false);
       public List<T> GetAll(Expression<Func<T, bool>> filter = null, string? includeprops = null);
       void Remove (T item);
       void RemoveRange(IEnumerable<T> items);
    }
}
