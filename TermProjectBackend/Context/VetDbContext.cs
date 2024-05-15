using Microsoft.EntityFrameworkCore;
using TermProjectBackend.Models;

namespace TermProjectBackend.Context
{
    public class VetDbContext : DbContext
    {
        public VetDbContext(DbContextOptions<VetDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<VetStaff> VetStaff { get; set;}

        public DbSet<Notification> Notification { get; set; }  

        public DbSet<Item> Items { get; set; }

        public DbSet<VaccinationRecord> VaccinationRecord {  get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<VeterinarianMessages> VeterinarianMessages { get; set; }


    }
}
