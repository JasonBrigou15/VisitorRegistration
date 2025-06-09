using VisitorRegistrationService.Dtos.Company;
using VisitorRegistrationService.Dtos.Employee;
using VisitorRegistrationShared.Extensions;

namespace VisitorRegistrationApi.Dtos.Company
{
    public static class CompanyMappingExtension
    {
        public static VisitorRegistrationData.Entities.Company CreateDtoToEntity(this CreateCompanyDto createCompanyDto)
        {
            var company = new VisitorRegistrationData.Entities.Company
            {
                Name = createCompanyDto.Name.ToTitleCase(),
            };

            return company;
        }

        public static void UpdateDtoToEntity(this UpdateCompanyDto updateCompanyDto, VisitorRegistrationData.Entities.Company company)
        {
            company.Name = updateCompanyDto.Name.ToTitleCase();
        }

        public static GetCompanyDto ToGetDto(this VisitorRegistrationData.Entities.Company company)
        {
            return new GetCompanyDto
            {
                CompanyId = company.Id,
                CompanyName = company.Name,
                Employees = company.Employees?.Select(e => new GetEmployeeDto
                {
                    EmployeeId = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Title = e.Title,
                    CompanyEmailAddress = e.CompanyEmail,
                    CompanyName = company.Name
                }).ToList() ?? new()
            };
        }
    }
}
