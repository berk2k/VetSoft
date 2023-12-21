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
    }
}
