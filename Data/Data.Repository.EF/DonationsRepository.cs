using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model.Donations;


namespace Data.Repository.EF
{
    public class DonationsRepository : IDonationsRepository
    {
        private DonationsDbContext _dbContext;

        public DonationsRepository(DonationsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddDonation(Donation donation)
        {
            _dbContext.Add(donation);
            await _dbContext.SaveChangesAsync();
        }

        public Task<List<Donation>> GetAllDonations()
        {
            return _dbContext.Set<Donation>().AsNoTracking().ToListAsync();
        }
    }
}