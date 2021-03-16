using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AlgorandCrowdfund.Models
{
    public class RequestFunds
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FundTitle { get; set; }
        [Required]
        public string FundDescription { get; set; }
        [Required]
        public int AmountNeeded { get; set; }
        [Required]
        public DateTime Created { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual IEnumerable<Funders> Funders { get; set; }
    }
}
