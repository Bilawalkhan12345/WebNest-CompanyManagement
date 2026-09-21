using Microsoft.EntityFrameworkCore;
using WebNest.Models;
namespace WebNest.Data
{

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Registration> Registration { get; set; }
        public DbSet<Companies> Companies { get; set; }
        public DbSet<AssignComp> AssignComp { get; set; }
        public DbSet<Admin> Admin { get; set; }

        public DbSet<Signup> Signup { get; set; }
    }
}
