using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cache;
using Contracts.Donation;
using Data.Repository;
using Model.Donations;

namespace Services.Donations
{
    public class DonationService : IDonationService
    {
        private const string TotalAmountKey = "TotalAmount";
        
        private readonly IDonationsRepository _repository;

        
        private ICacheService _cacheService;

        public DonationService(IDonationsRepository repository, ICacheService cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task<int> GetTotalDonationsAmount()
        {
            var cachedValue = await _cacheService.Get<int>(TotalAmountKey);
            if (cachedValue.Exists)
            {
                return cachedValue.Value;
            }
            else
            {
                var donations = await _repository.GetAllDonations();
                var total = (int)donations.Sum(d => d.Amount);
                await _cacheService.Set(TotalAmountKey, total, TimeSpan.FromSeconds(30));
                return total;
            }
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