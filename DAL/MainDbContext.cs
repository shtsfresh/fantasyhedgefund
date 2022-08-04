using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class MainDbContext : IdentityDbContext<IdentityUser>
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        public DbSet<AssetDAL> Assets { get; set; }
        public DbSet<TransactionDAL> Transactions { get; set; }
        public DbSet<HoldingDAL> Holdings { get; set; }
    }
}