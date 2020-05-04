using System;
using System.Threading.Tasks;
using Contracts.Donation;
using Model.Donations;

namespace Services.Donations
{
    public class DonationService : IDonationService
    {
        public Task<int> GetTotalDonationsAmount()
        {
            return Task.FromResult(233);
        }

        public Task Donate(Donation donation)
        {
            throw new NotImplementedException();
        }
    }
}