namespace VisitorRegistrationShared.Dtos.Employee
{
    public class CreateEmployeeDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int CompanyId { get; set; } = 0;
    }
}
