namespace Stockwinners
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public abstract class IDatabaseContext : DbContext
    {
        public IDatabaseContext(string nameOrConnectionString) : base(nameOrConnectionString) 
        {
        }

        public abstract IQueryable<IUser> GetUsers { get; }
    }
}
