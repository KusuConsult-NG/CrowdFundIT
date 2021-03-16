using System.ComponentModel.DataAnnotations;

namespace AlgorandCrowdfund.Models
{
    public class Funders
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int Amount { get; set; }
        public string Key { get; set; }
        public string Receiver { get; set; }
        public virtual ApplicationUser User { get; set; }
        public int RequestFundsId { get; set; }
        public RequestFunds RequestFunds { get; set; }
    }
}
