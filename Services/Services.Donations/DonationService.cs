using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Donation;
using Data.Repository;
using Model.Donations;

namespace Services.Donations
{
    public class DonationService : IDonationService
    {
        private IDonationsRepository _repository;

        public DonationService(IDonationsRepository repository)
        {
            _repository = repository;
        }

        public Task<int> GetTotalDonationsAmount()
        {
            return Task.FromResult(233);
        }

        public async Task Donate(Donation donation)
        {
            await _repository.AddDonation(donation);
        }

        public async Task<List<Donation>> GetAllDonations()
        {
            return await _repository.GetAllDonations();
        }
    }
}