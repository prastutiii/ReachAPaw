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
        public DbSet<PetModel> Pets { get; set; }
        public DbSet<ShelterModel> Shelters { get; set; }
        public DbSet<AdoptionApplicationModel> AdoptionApplications { get; set; }
        public DbSet<ApplicationDetailsModel> ApplicationDetails { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        public DbSet<AdoptionModel> Adoptions { get; set; }
        public DbSet<FavoriteModel> Favorites { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<VisitModel> Visits { get; set; }
        public DbSet<CommunityModel> Community { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AdoptionApplicationModel>()
                .HasOne(a => a.ApplicationDetails)
                .WithOne(d => d.AdoptionApplications)
                .HasForeignKey<AdoptionApplicationModel>(a => a.application_id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
