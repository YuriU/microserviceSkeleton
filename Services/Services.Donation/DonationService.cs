using System;
using System.Threading.Tasks;
using Contracts.Donation;

namespace Services.Donation
{
    public class DonationService : IDonationService
    {
        public Task<int> GetTotalDonationsAmount()
        {
            return Task.FromResult(233);
        }

        public Task Donate(Contracts.Donation.Donation donation)
        {
            throw new NotImplementedException();
        }
    }
}