using ecommerce.Dataaccess.Data;
using ecommerce.Dataaccess.Repo.IRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Repo
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly Ecommdbcontext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(Ecommdbcontext context)
        {
                this._context = context;
            this._dbSet =   context.Set<T>();
        }
        public void Add(T item)
        {           
        _dbSet.Add(item);
        }

        public T Get(Expression<Func<T, bool>> filter, string? includeprops = null, bool tracking = false)
        {
            IQueryable<T> query;
           
                if (tracking == true)
                {
                    query = _dbSet.Where(filter);
                }
                else
                {
                    query = _dbSet.AsNoTracking<T>();
                }
                query = query.Where(filter);

                if (includeprops != null)
                {
                    foreach (var item in includeprops.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {

                        query = query.Include(item);
                    }
                }

            
            return query.FirstOrDefault();
            
        }

        public List<T> GetAll(Expression<Func<T, bool>> filter = null, string? includeprops = null)
        {
            IQueryable<T>query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
           

            if (includeprops != null)
            {
                foreach (var item in includeprops.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {

                    query = query.Include(item);
                }
            }
            return query.ToList();

        }

        public void Remove(T item)
        {
            _dbSet.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            _dbSet.RemoveRange(items);
        }
    }
}
