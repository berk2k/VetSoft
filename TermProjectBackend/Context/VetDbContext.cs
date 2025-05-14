using Microsoft.EntityFrameworkCore;
using TermProjectBackend.Models;

namespace TermProjectBackend.Context
{
    public class VetDbContext : DbContext
    {
        public VetDbContext()
        {
            
        }
        public VetDbContext(DbContextOptions<VetDbContext> options)
            : base(options)
        {

        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Pet> Pets { get; set; }

        public virtual DbSet<Appointment> Appointments { get; set; }

        public virtual DbSet<VetStaff> VetStaff { get; set;}

        public virtual DbSet<Notification> Notification { get; set; }  

        public virtual DbSet<Item> Items { get; set; }

        public virtual DbSet<VaccinationRecord> VaccinationRecord {  get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<VeterinarianMessages> VeterinarianMessages { get; set; }

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.AppUser)
                .WithMany() // Specify related navigation property in ApplicationUser if needed
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete tokens when user is deleted
        }


    }
}
