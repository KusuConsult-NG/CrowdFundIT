using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AlgorandCrowdfund.Models;

namespace AlgorandCrowdfund.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<AlgorandCrowdfund.Models.RequestFunds> RequestFunds { get; set; }
        public DbSet<AlgorandCrowdfund.Models.Funders> Funders { get; set; }
    }
}
