namespace VisitorRegistrationData.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string CompanyEmail { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public required Company Company { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
