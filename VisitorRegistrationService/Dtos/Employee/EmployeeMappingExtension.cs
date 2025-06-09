using VisitorRegistrationData.Entities;
using VisitorRegistrationShared.Extensions;

namespace VisitorRegistrationService.Dtos.Employee
{
    public static class EmployeeMappingExtension
    {
        public static VisitorRegistrationData.Entities.Employee CreateDtoToEntity(this CreateEmployeeDto createEmployeeDto, VisitorRegistrationData.Entities.Company company)
        {
            var employee = new VisitorRegistrationData.Entities.Employee
            {
                FirstName = createEmployeeDto.FirstName.ToTitleCase(),
                LastName = createEmployeeDto.LastName.ToTitleCase(),
                Title = createEmployeeDto.Title.ToTitleCase(),
                Company = company,
                CompanyId = company.Id,
                IsDeleted = false
                
            };

            employee.CompanyEmail = $"{createEmployeeDto.FirstName.NormalizeForComparison()}.{createEmployeeDto.LastName.NormalizeForComparison()}" +
                $".{createEmployeeDto.Title.NormalizeForComparison()}@{company.Name.NormalizeForComparison()}.com";

            return employee;
        }

        public static void UpdateDtoToEntity(this UpdateEmployeeDto updateEmployeeDto, VisitorRegistrationData.Entities.Employee employee, VisitorRegistrationData.Entities.Company company)
        {
            employee.FirstName = updateEmployeeDto.FirstName.ToTitleCase();
            employee.LastName = updateEmployeeDto.LastName.ToTitleCase();
            employee.Title = updateEmployeeDto.Title.ToTitleCase();
            employee.Company = company;
            employee.CompanyId = company.Id;
            employee.IsDeleted = false;
            employee.CompanyEmail = $"{updateEmployeeDto.FirstName.NormalizeForComparison()}.{updateEmployeeDto.LastName.NormalizeForComparison()}" +
                $".{updateEmployeeDto.Title.NormalizeForComparison()}@{company.Name.NormalizeForComparison()}.com";
        }

        public static GetEmployeeDto ToGetDto(this VisitorRegistrationData.Entities.Employee employee)
        {
            return new GetEmployeeDto
            {
                EmployeeId = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Title = employee.Title,
                CompanyEmailAddress = employee.CompanyEmail,
                CompanyName = employee.Company.Name,
            };
        }
    }
}