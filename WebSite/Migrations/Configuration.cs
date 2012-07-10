namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
using WebSite.Database;
    using WebSite.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DatabaseContext context)
        {
            // This method will be called after migrating to the latest version.

            // Ensure default subscriptions are available
            context.Subscriptions.AddOrUpdate(
                new Subscription() { Frequency = SubscriptionFrequency.Monthly, Price = 39 },
                new Subscription() { Frequency = SubscriptionFrequency.Quarterly, Price = 110 },
                new Subscription() { Frequency = SubscriptionFrequency.Yearly, Price = 350 }
                );
        }
    }
}
