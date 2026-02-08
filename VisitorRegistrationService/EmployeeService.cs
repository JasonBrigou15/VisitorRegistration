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

        public async Task<List<Employee>> GetEmployeesByCompanyId(int companyId)
        {
            return await employeeRepository.GetAllEmployeesByCompany(companyId);
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

            Console.WriteLine($"Checking email: {updateEmployeeDto.CompanyEmail}");  // ADD THIS

            var employeeWithSameEmail = await employeeRepository.GetEmployeeByEmail(updateEmployeeDto.CompanyEmail);

            Console.WriteLine($"Found employee: {employeeWithSameEmail?.Id}, Current ID: {updateEmployeeDto.Id}");  // ADD THIS

            if (employeeWithSameEmail != null && employeeWithSameEmail.Id != updateEmployeeDto.Id)
                throw new ValidationException("An employee with this email already exists");

            var company = await companyRepository.GetCompanyById(updateEmployeeDto.CompanyId);
            if (company == null)
                throw new Exception($"Company with ID {updateEmployeeDto.CompanyId} not found");

            updateEmployeeDto.UpdateDtoToEntity(existingEmployee, company);
            await employeeRepository.UpdateEmployee(existingEmployee);
        }

        public async Task DeleteEmployee(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid employee ID");

            var existingEmployee = await employeeRepository.GetEmployeeById(id);

            if (existingEmployee == null)
                throw new Exception($"Employee with ID {id} not found");

            await employeeRepository.DeleteEmployee(id);
        }
    }
}
