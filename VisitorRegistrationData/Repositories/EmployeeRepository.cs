using Microsoft.EntityFrameworkCore;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;

namespace VisitorRegistrationData.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly VisitorRegistrationDbContext context;

        public EmployeeRepository(VisitorRegistrationDbContext context)
        {
            this.context = context;
        }

        public async Task<Employee> CreateEmployee(Employee employee)
        {
            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();
            return employee;
        }

        public async Task DeleteEmployee(int id)
        {
            var employee = await context.Employees.FindAsync(id);
            employee!.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<List<Employee>> GetAllEmployees() => await context.Employees
            .Include(e => e.Company)
            .Where(e => !e.IsDeleted)
            .ToListAsync();

        public Task<List<Employee>> GetAllEmployeesByCompany(int companyId) => context.Employees
            .Include(e => e.Company)
            .Where(e => !e.IsDeleted && e.CompanyId == companyId)
            .ToListAsync();

        public async Task<Employee?> GetEmployeeById(int id) => await context.Employees
            .Include(e => e.Company)
            .Where(e => !e.IsDeleted)
            .SingleOrDefaultAsync(e => e.Id == id);

        public async Task UpdateEmployee(Employee employee)
        {
            var existingEmployee = await context.Employees.FindAsync(employee.Id);

            context.Entry(existingEmployee!).CurrentValues.SetValues(employee);

            await context.SaveChangesAsync();
        }
    }
}
