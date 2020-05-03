using System;

namespace Contracts.Donation
{
    public class Donation
    {
        public int Id { get; set; }
        
        public string DonorName { get; set; }
        
        public decimal Amount { get; set; }
        
        public DateTime Date { get; set; }
    }
}