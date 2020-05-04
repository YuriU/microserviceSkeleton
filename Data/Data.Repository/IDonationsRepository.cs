using System.Collections.Generic;
using System.Threading.Tasks;
using Model.Donations;

namespace Data.Repository
{
    public interface IDonationsRepository
    {
        Task AddDonation(Donation donation);

        Task<List<Donation>> GetAllDonations();
    }
}