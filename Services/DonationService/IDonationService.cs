using System.Threading.Tasks;
using DonationService.Contracts;

namespace DonationService
{
    public interface IDonationService
    {
        Task<int> GetTotalDonationsAmount();

        Task Donate(Donation donation);
    }
}