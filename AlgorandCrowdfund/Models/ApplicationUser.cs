using Microsoft.AspNetCore.Identity;

namespace AlgorandCrowdfund.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string AccountAddress { get; set; }
        public string Key { get; set; }
    }
}
