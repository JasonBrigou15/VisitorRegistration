using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;

namespace VisitorRegistrationService
{
    public class CompanyService
    {
        private readonly ICompanyRepository companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;
        }

        public async Task<List<Company>> GetAllCompanies()
        {
            return await companyRepository.GetAllCompanies();
        }

        public async Task<Company?> GetCompanyById(int id)
        {
            return await companyRepository.GetCompanyById(id);
        }

        public async Task<Company?> GetCompanyByName(string name)
        {
            return await companyRepository.GetCompanyByName(name);
        }

        public async Task CreateNewCompany(Company company)
        {
            await companyRepository.CreateCompany(company);
        }

        public async Task UpdateCompany(Company company)
        {
            await companyRepository.UpdateCompany(company);
        }

        public async Task DeleteCompany(int id)
        {
            await companyRepository.DeleteCompany(id);
        }
    }
}
