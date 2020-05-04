using System.Threading.Tasks;

namespace Contracts.Donation
{
    public interface IDonationService
    {
        Task<int> GetTotalDonationsAmount();

        Task Donate(Model.Donations.Donation donation);
    }
}