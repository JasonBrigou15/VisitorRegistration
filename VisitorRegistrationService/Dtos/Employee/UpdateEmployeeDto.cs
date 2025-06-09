namespace VisitorRegistrationService.Dtos.Employee
{
    public class UpdateEmployeeDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Title {  get; set; } = string.Empty;

        public int CompanyId { get; set; } = 0;
    }
}
