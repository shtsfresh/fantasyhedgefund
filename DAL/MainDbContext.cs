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
    }
}