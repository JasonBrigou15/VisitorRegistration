using FluentValidation;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationService.Dtos.Employee;
using VisitorRegistrationShared.Dtos.Employee;

namespace VisitorRegistrationService
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly ICompanyRepository companyRepository;

        // Validator
        private readonly IValidator<CreateEmployeeDto> createEmployeeDto;
        private readonly IValidator<UpdateEmployeeDto> updateEmployeeDto;

        public EmployeeService(IEmployeeRepository employeeRepository, ICompanyRepository companyRepository,
            IValidator<CreateEmployeeDto> createEmployeeDto, IValidator<UpdateEmployeeDto> updateEmployeeDto)
        {
            this.employeeRepository = employeeRepository;
            this.companyRepository = companyRepository;
            this.createEmployeeDto = createEmployeeDto;
            this.updateEmployeeDto = updateEmployeeDto;
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            return await employeeRepository.GetAllEmployees();
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await employeeRepository.GetEmployeeById(id);
        }

        public async Task CreateNewEmployee(CreateEmployeeDto createEmployeeDto)
        {
            var validationResult = await this.createEmployeeDto.ValidateAsync(createEmployeeDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var company = await companyRepository.GetCompanyById(createEmployeeDto.CompanyId);

            if (company == null)
                throw new ArgumentException("Selected company does not exist.");

            var employee = createEmployeeDto.CreateDtoToEntity(company);

            await employeeRepository.CreateEmployee(employee);
        }

        public async Task UpdateEmployee(UpdateEmployeeDto updateEmployeeDto)
        {
            var validationResult = await this.updateEmployeeDto.ValidateAsync(updateEmployeeDto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingEmployee = await employeeRepository.GetEmployeeById(updateEmployeeDto.Id);

            if (existingEmployee == null)
                throw new Exception($"Employee with ID {updateEmployeeDto.Id} not found");

            var company = await companyRepository.GetCompanyById(updateEmployeeDto.CompanyId);

            if (company == null)
                throw new Exception($"Company with ID {updateEmployeeDto.CompanyId} not found");

            updateEmployeeDto.UpdateDtoToEntity(existingEmployee, company);

            await employeeRepository.UpdateEmployee(existingEmployee);
        }

        public async Task DeleteEmployee(int id)
        {
            if (id < 0)
                throw new Exception("Invalid employee ID");

            await employeeRepository.DeleteEmployee(id);
        }
    }
}
