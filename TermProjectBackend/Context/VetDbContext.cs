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
        public DbSet<Pet> Pets { get; set; }

        public virtual DbSet<Appointment> Appointments { get; set; }

        public DbSet<VetStaff> VetStaff { get; set;}

        public virtual DbSet<Notification> Notification { get; set; }  

        public virtual DbSet<Item> Items { get; set; }

        public DbSet<VaccinationRecord> VaccinationRecord {  get; set; }

        public DbSet<Review> Reviews { get; set; }

        public virtual DbSet<VeterinarianMessages> VeterinarianMessages { get; set; }


    }
}
