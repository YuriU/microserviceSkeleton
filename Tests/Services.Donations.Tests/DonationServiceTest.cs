using System.Threading.Tasks;
using Model.Donations;
using NUnit.Framework;

namespace Services.Donations.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task RepositoryIsEmpty()
        {
            var service = new DonationService(new MockRepository(), new MockCacheService());

            var donations = await service.GetAllDonations();
            
            Assert.IsEmpty(donations);
        }
        
        [Test]
        public async Task AddedItemsAmountMatches()
        {
            var service = new DonationService(new MockRepository(), new MockCacheService());

            await service.Donate(new Donation { Amount = 500 });
            await service.Donate(new Donation { Amount = 150 });
                
            var totalDonationsAmount = await service.GetTotalDonationsAmount();

            Assert.AreEqual(650, totalDonationsAmount);
        }
    }
}