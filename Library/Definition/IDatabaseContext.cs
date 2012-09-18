namespace Stockwinners
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    public abstract class IDatabaseContext : DbContext
    {
        public IDatabaseContext(string nameOrConnectionString) : base(nameOrConnectionString) 
        {
        }

        public abstract IEnumerable<IUser> GetUsers { get; }
    }
}
