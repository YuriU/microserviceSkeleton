using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts.Donation;
using Microsoft.AspNetCore.Mvc;
using Model.Donations;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DonationController
    {
        private IDonationService _donationService;

        public DonationController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        [HttpGet("totalAmount")]
        public Task<int> GetTotalAmount()
        {
            return _donationService.GetTotalDonationsAmount();
        }
        
        [HttpGet("donations")]
        public async Task<List<Donation>> GetAllDonations()
        {
            return await _donationService.GetAllDonations();
        }

        [HttpPost]
        public Task MakeDonation(Donation donation)
        {
            return _donationService.Donate(donation);
        }
    }
}