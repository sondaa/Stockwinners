using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebSite.Models;

namespace WebSite.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<StockwinnersMember> StockwinnersMembers { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }

        #region Lifetime Management

        public static bool IsAvailableForCurrentContext()
        {
            return HttpContext.Current.Items.Contains(typeof(DatabaseContext));
        }

        public static DatabaseContext GetInstance()
        {
            if (DatabaseContext.IsAvailableForCurrentContext())
            {
                return HttpContext.Current.Items[typeof(DatabaseContext)] as DatabaseContext;
            }
            else
            {
                DatabaseContext db = new DatabaseContext();

                HttpContext.Current.Items.Add(typeof(DatabaseContext), db);

                return db;
            }
        }

        public static void DisposeInstance()
        {
            if (DatabaseContext.IsAvailableForCurrentContext())
            {
                DatabaseContext.GetInstance().Dispose();
            }
        }

        #endregion
    }
}