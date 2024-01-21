using ecommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecommerce.Dataaccess.Data
{
    public class Ecommdbcontext : IdentityDbContext
    {
        public Ecommdbcontext(DbContextOptions<Ecommdbcontext>options):base(options) 
        {
            
        }
        public DbSet<Product> products { get; set; }
        public DbSet<Images> images { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<OrderDetail> ordersDetail { get; set; }
        public DbSet<Customer> customers { get; set; }
        public DbSet<AppUser> appUsers { get; set; }


    }
}
   
