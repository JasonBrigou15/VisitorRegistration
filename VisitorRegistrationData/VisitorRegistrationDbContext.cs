using Microsoft.EntityFrameworkCore;
using VisitorRegistrationData.Configurations;
using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData
{
    public class VisitorRegistrationDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; } = null!;

        public DbSet<Employee> Employees { get; set; }

        public VisitorRegistrationDbContext() { }

        public VisitorRegistrationDbContext(DbContextOptions<VisitorRegistrationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());

            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
