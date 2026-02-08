namespace VisitorRegistrationShared.Dtos.Employee
{
    public class GetEmployeeDto
    {
        public int EmployeeId { get; set; } = 0;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string CompanyEmail { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public string CompanyEmailAddress { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
}
