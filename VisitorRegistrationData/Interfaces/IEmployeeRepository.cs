using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllEmployees();

        Task<Employee?> GetEmployeeById(int id);

        Task<Employee?> GetEmployeeByEmail(string email);

        Task<List<Employee>> GetAllEmployeesByCompany(int companyId);

        Task<Employee> CreateEmployee(Employee employee);

        Task UpdateEmployee(Employee employee);

        Task DeleteEmployee(int id);
    }
}
