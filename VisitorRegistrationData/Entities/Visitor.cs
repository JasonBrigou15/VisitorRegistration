namespace VisitorRegistrationData.Entities
{
    public class Visitor
    {
        public int Id { get; set; }

        public required string Firstname { get; set; }

        public required string Lastname { get; set; }

        public required string Email { get; set; }

        public int? CompanyId { get; set; }

        public string? CompanyName { get; set; }

        public Company? Company { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
