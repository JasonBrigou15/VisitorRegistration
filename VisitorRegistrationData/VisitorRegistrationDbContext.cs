using Microsoft.EntityFrameworkCore;
using VisitorRegistrationData.Configurations;
using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData
{
    public class VisitorRegistrationDbContext : DbContext
    {
        public VisitorRegistrationDbContext(DbContextOptions<VisitorRegistrationDbContext> options) : base(options) { }

        public DbSet<Company> Companies { get; set; } = null!;

        public DbSet<Employee> Employees { get; set; } = null!;

        public DbSet<Visitor> Visitors { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());

            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());

            modelBuilder.ApplyConfiguration(new VisitorConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
