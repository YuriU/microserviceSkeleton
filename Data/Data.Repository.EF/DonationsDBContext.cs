using System;
using Microsoft.EntityFrameworkCore;
using Model.Donations;

namespace Data.Repository.EF
{
    public class DonationsDbContext : DbContext
    {
        private readonly Action<DbContextOptionsBuilder> _builder;
        
        public DonationsDbContext(Action<DbContextOptionsBuilder> builder)
        {
            _builder = builder;
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Referencing donation object for the simplicity sake
            modelBuilder.Entity<Donation>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (this._builder != null)
            {
                _builder(optionsBuilder);
            }
        }
    }
}