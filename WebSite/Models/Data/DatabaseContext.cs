using Stockwinners;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebSite.Models;
using WebSite.Models.Data;
using WebSite.Models.Data.Picks;

namespace WebSite.Database
{
    public class DatabaseContext : Stockwinners.IDatabaseContext
    {
        public DatabaseContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionType> SubscriptionTypes { get; set; }
        public DbSet<SubscriptionFrequency> SubscriptionFrequencies { get; set; }
        public DbSet<StockwinnersMember> StockwinnersMembers { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<DailyAlert> DailyAlerts { get; set; }
        public DbSet<Figure> Figures { get; set; }

        // Picks related
        public DbSet<Pick> Picks { get; set; }
        public DbSet<PickUpdate> PickUpdates { get; set; }
        public DbSet<OptionPick> OptionPicks { get; set; }
        public DbSet<OptionPickType> OptionPickTypes { get; set; }
        public DbSet<OptionPickLeg> OptionPickLegs { get; set; }
        public DbSet<StockPick> StockPicks { get; set; }
        public DbSet<StockPickType> StockPickTypes { get; set; }

        public override IQueryable<IUser> GetUsers { get { return this.Users; } }
    }
}