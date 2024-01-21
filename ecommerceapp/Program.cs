using Microsoft.EntityFrameworkCore;
using ecommerce.Dataaccess;
using ecommerce.Dataaccess.Data;
using Microsoft.Extensions.Options;
using ecommerce.Dataaccess.Repo.IRepo;
using ecommerce.Dataaccess.Repo;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ecommerce.Utilities;

namespace ecommerceapp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
           
            builder.Services.AddDbContext<Ecommdbcontext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
               
        });
            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<Ecommdbcontext>().AddDefaultTokenProviders();

            
           
            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10000);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddRazorPages();
            builder.Services.AddScoped<IUnitOfwork, UnitOfwork>();
            builder.Services.AddScoped<IEmailSender,EmailSender>();
            builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages(); 
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}