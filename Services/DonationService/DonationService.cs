using System.Threading.Tasks;
using DonationService.Contracts;

namespace DonationService
{
    public class DonationService : IDonationService
    {
        public Task<int> GetTotalDonationsAmount()
        {
            throw new System.NotImplementedException();
        }

        public Task Donate(Donation donation)
        {
            throw new System.NotImplementedException();
        }
    }
}