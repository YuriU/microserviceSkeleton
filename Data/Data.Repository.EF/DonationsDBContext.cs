using Contracts.Donation;
using Microsoft.EntityFrameworkCore;

namespace Data.Repository.EF
{
    public class DonationsDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Referencing donation object for the simplicity sake
            modelBuilder.Entity<Donation>();
        }
    }
}