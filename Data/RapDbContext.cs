using Microsoft.EntityFrameworkCore;
using ReachAPaw.Models;

namespace ReachAPaw.Data
{
    public class RapDbContext: DbContext
    {
        public RapDbContext(DbContextOptions<RapDbContext> options) : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }

    }
}
