using Microsoft.EntityFrameworkCore;
using PgConsoleCrud.Models;

namespace PgConsoleCrud.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TestSaurav;Username=postgres;Password=tech");
        }
    }
}
