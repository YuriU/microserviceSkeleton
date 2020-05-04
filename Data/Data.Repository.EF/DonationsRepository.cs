using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Donations;

namespace Data.Repository.EF
{
    public class DonationsRepository : IDonationsRepository
    {
        public Task AddDonation(Donation donation)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Donation>> GetAllDonations()
        {
            throw new System.NotImplementedException();
        }
    }
}