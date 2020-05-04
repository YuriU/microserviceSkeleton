using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Repository;
using Model.Donations;

namespace Services.Donations.Tests
{
    public class MockRepository : IDonationsRepository
    {
        public List<Donation> _donations = new List<Donation>();
        
        public Task AddDonation(Donation donation)
        {
            if (donation.Id > 0)
            {
                var index = _donations.FindIndex(d => d.Id == donation.Id);
                if (index == -1)
                {
                    _donations.Add(donation);
                }
                else
                {
                    _donations[index] = donation;
                }
            }
            else
            {
                _donations.Add(donation);
            }

            return Task.CompletedTask;
        }

        public Task<List<Donation>> GetAllDonations()
        {
            return Task.FromResult(_donations.ToList());
        }
    }
}