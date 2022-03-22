using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=RunDB;Integrated Security=True;");
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Run> Runs { get; set; }

        public DbSet<Point> Points { get; set; }
    }
}
