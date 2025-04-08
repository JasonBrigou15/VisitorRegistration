using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData.Interfaces
{
    public interface ICompanyRepository
    {
        Task<List<Company>> GetAllCompanies();

        Task<Company?> GetCompanyById(int id);

        Task<Company> CreateCompany(Company company);

        Task UpdateCompany(Company company);

        Task DeleteCompany(int id);
    }
}
