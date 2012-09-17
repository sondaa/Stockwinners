namespace Stockwinners
{
    using System;
    using System.Data.Entity;

    public abstract class IDatabaseContext : DbContext
    {
        public IDatabaseContext(string nameOrConnectionString) : base(nameOrConnectionString) 
        {
        }

        System.Data.Entity.DbSet<IUser> Users { get; set; }
    }
}
