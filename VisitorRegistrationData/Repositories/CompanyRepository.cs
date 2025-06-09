using Microsoft.EntityFrameworkCore;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;

namespace VisitorRegistrationData.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly VisitorRegistrationDbContext context;

        public CompanyRepository(VisitorRegistrationDbContext context)
        {
            this.context = context;
        }

        public async Task<Company> CreateCompany(Company company)
        {
            await context.Companies.AddAsync(company);
            await context.SaveChangesAsync();
            return company;
        }

        public async Task DeleteCompany(int id)
        {
            var company = await context.Companies.FindAsync(id);
            company!.IsDeleted = true;
            await context.SaveChangesAsync();
        }

        public async Task<List<Company>> GetAllCompanies() => await context.Companies
            .Include(c => c.Employees)
            .Where(c => !c.IsDeleted)
            .ToListAsync();

        public async Task<Company?> GetCompanyById(int id) => await context.Companies
            .Include(c => c.Employees)
            .Where(c => !c.IsDeleted)
            .SingleOrDefaultAsync(c => c.Id == id);

        public async Task UpdateCompany(Company company)
        {
            var existingCompany = await context.Companies.FindAsync(company.Id);

            context.Entry(existingCompany!).CurrentValues.SetValues(company);

            await context.SaveChangesAsync();
        }
    }
}
