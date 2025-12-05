using VisitorRegistrationShared.Dtos.Employee;

namespace VisitorRegistrationShared.Dtos.Company
{
    public class GetCompanyDto
    {
        public int CompanyId { get; set; } = 0;
        public string CompanyName { get; set; } = string.Empty;
        public List<GetEmployeeDto> Employees { get; set; } = new List<GetEmployeeDto>();
    }
}
